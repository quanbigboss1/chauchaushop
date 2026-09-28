using ChauChau.Context;
using ChauChau.Models;
using ChauChau.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;
    private readonly IVNPayService _vnPay;
    private readonly IMomoService _momoService;

    public CheckoutController(
        ApplicationDbContext db,
        IHttpContextAccessor http,
        IVNPayService vnPay,
        IMomoService momoService)
    {
        _db = db;
        _http = http;
        _vnPay = vnPay;
        _momoService = momoService;
    }

    // =========================================================
    // LẤY USER ID
    // =========================================================
    private int? GetUserId()
    {
        var claim = _http.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        return int.TryParse(claim, out int id)
            ? id
            : (int?)null;
    }

    // =========================================================
    // CHECKOUT
    // =========================================================
    [HttpGet]
    public IActionResult Index()
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = _db.GioHangs
            .Include(g => g.SanPhamVariant)
            .ThenInclude(v => v.SanPham)
            .Where(g => g.UserId == userId.Value)
            .ToList();

        if (!cart.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        ViewBag.Total = cart.Sum(
            x => x.SoLuong * x.SanPhamVariant.Gia
        );

        return View(cart);
    }

    // =========================================================
    // VNPay - TẠO ĐƠN HÀNG
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PayOnlineWithVNPay()
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = _db.GioHangs
            .Include(g => g.SanPhamVariant)
            .Where(g => g.UserId == userId.Value)
            .ToList();

        if (!cart.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        // =====================================================
        // TÍNH TỔNG TIỀN
        // =====================================================

        decimal total = cart.Sum(
            x => x.SoLuong * x.SanPhamVariant.Gia
        );

        if (total <= 0)
        {
            TempData["PaymentError"] =
                "Số tiền thanh toán không hợp lệ.";

            return RedirectToAction("Index");
        }

        // =====================================================
        // KIỂM TRA TỒN KHO TRƯỚC KHI TẠO ORDER
        // =====================================================

        foreach (var item in cart)
        {
            if (item.SanPhamVariant == null)
                continue;

            if (item.SanPhamVariant.SoLuong < item.SoLuong)
            {
                TempData["PaymentError"] =
                    $"Sản phẩm không đủ số lượng trong kho.";

                return RedirectToAction("Index");
            }
        }

        // =====================================================
        // TẠO ORDER
        // =====================================================

        var order = new DonHang
        {
            UserId = userId.Value,
            NgayDat = DateTime.Now,
            NgayTao = DateTime.Now,
            TongTien = total,
            TrangThai = "ChoThanhToan"
        };

        _db.DonHangs.Add(order);

        _db.SaveChanges();

        // =====================================================
        // TẠO CHI TIẾT ORDER
        // =====================================================

        foreach (var item in cart)
        {
            _db.ChiTietDonHangs.Add(
                new ChiTietDonHang
                {
                    DonHangId = order.DonHangId,
                    SanPhamVariantId = item.SanPhamVariantId,
                    SoLuong = item.SoLuong,
                    Gia = item.SanPhamVariant.Gia
                }
            );
        }

        _db.SaveChanges();

        // =====================================================
        // TẠO URL VNPAY
        // =====================================================

        string paymentUrl =
            _vnPay.CreatePaymentUrl(
                HttpContext,
                total,
                order.DonHangId
            );

        return Redirect(paymentUrl);
    }

    // =========================================================
    // VNPAY RETURN
    // URL:
    // /Checkout/VNPayReturn
    // =========================================================

    [HttpGet]
    public IActionResult VNPayReturn()
    {
        try
        {
            // =================================================
            // XỬ LÝ CALLBACK
            // =================================================

            var response =
                _vnPay.ProcessReturn(Request.Query);

            if (response.OrderId <= 0)
            {
                return View("PaymentFail");
            }

            // =================================================
            // TÌM ORDER
            // =================================================

            var order = _db.DonHangs
                .Include(o => o.ChiTietDonHangs)
                .ThenInclude(c => c.SanPhamVariant)
                .FirstOrDefault(
                    o => o.DonHangId == response.OrderId
                );

            if (order == null)
            {
                return View("PaymentFail");
            }

            // =================================================
            // NẾU ĐÃ THANH TOÁN
            // KHÔNG XỬ LÝ LẠI
            // =================================================

            if (order.TrangThai == "DaThanhToan")
            {
                return RedirectToAction(
                    "Success",
                    new { id = order.DonHangId }
                );
            }

            // =================================================
            // KIỂM TRA SỐ TIỀN
            // =================================================

            if (response.Amount != order.TongTien)
            {
                order.TrangThai = "ThanhToanThatBai";

                _db.SaveChanges();

                return View("PaymentFail");
            }

            // =================================================
            // THANH TOÁN THÀNH CÔNG
            // =================================================

            if (response.Success)
            {
                // ---------------------------------------------
                // Kiểm tra tồn kho lần cuối
                // ---------------------------------------------

                foreach (var item in order.ChiTietDonHangs)
                {
                    if (item.SanPhamVariant == null)
                        continue;

                    if (item.SanPhamVariant.SoLuong < item.SoLuong)
                    {
                        order.TrangThai =
                            "ThanhToanThatBai";

                        _db.SaveChanges();

                        return View("PaymentFail");
                    }
                }

                // ---------------------------------------------
                // TRỪ KHO
                // ---------------------------------------------

                foreach (var item in order.ChiTietDonHangs)
                {
                    if (item.SanPhamVariant != null)
                    {
                        item.SanPhamVariant.SoLuong -=
                            item.SoLuong;
                    }
                }

                // ---------------------------------------------
                // CẬP NHẬT ORDER
                // ---------------------------------------------

                order.TrangThai = "DaThanhToan";

                // ---------------------------------------------
                // XÓA CART CỦA USER
                // ---------------------------------------------

                var cart = _db.GioHangs
                    .Where(g =>
                        g.UserId == order.UserId
                    )
                    .ToList();

                if (cart.Any())
                {
                    _db.GioHangs.RemoveRange(cart);
                }

                _db.SaveChanges();

                return RedirectToAction(
                    "Success",
                    new { id = order.DonHangId }
                );
            }

            // =================================================
            // THANH TOÁN THẤT BẠI
            // =================================================

            order.TrangThai =
                "ThanhToanThatBai";

            _db.SaveChanges();

            return View("PaymentFail");
        }
        catch (Exception ex)
        {
            // Log lỗi nếu cần
            Console.WriteLine(
                "VNPay Return Error: " +
                ex.Message
            );

            return View("PaymentFail");
        }
    }

    // =========================================================
    // VNPAY IPN
    // URL:
    // /Checkout/VNPayIPN
    //
    // Dùng để cấu hình trong Merchant Admin VNPay
    // =========================================================

    [HttpGet]
    public IActionResult VNPayIPN()
    {
        try
        {
            var response =
                _vnPay.ProcessReturn(Request.Query);

            if (response.OrderId <= 0)
            {
                return Json(new
                {
                    RspCode = "01",
                    Message = "Order not found"
                });
            }

            var order = _db.DonHangs
                .Include(o => o.ChiTietDonHangs)
                .ThenInclude(c => c.SanPhamVariant)
                .FirstOrDefault(
                    o => o.DonHangId == response.OrderId
                );

            if (order == null)
            {
                return Json(new
                {
                    RspCode = "01",
                    Message = "Order not found"
                });
            }

            // =================================================
            // ĐÃ THANH TOÁN TRƯỚC ĐÓ
            // =================================================

            if (order.TrangThai == "DaThanhToan")
            {
                return Json(new
                {
                    RspCode = "00",
                    Message = "Confirm Success"
                });
            }

            // =================================================
            // KIỂM TRA SỐ TIỀN
            // =================================================

            if (response.Amount != order.TongTien)
            {
                return Json(new
                {
                    RspCode = "04",
                    Message = "Invalid amount"
                });
            }

            // =================================================
            // THANH TOÁN THÀNH CÔNG
            // =================================================

            if (response.Success)
            {
                // Kiểm tra kho
                foreach (var item in order.ChiTietDonHangs)
                {
                    if (item.SanPhamVariant == null)
                        continue;

                    if (item.SanPhamVariant.SoLuong <
                        item.SoLuong)
                    {
                        return Json(new
                        {
                            RspCode = "99",
                            Message = "Insufficient stock"
                        });
                    }
                }

                // Trừ kho
                foreach (var item in order.ChiTietDonHangs)
                {
                    if (item.SanPhamVariant != null)
                    {
                        item.SanPhamVariant.SoLuong -=
                            item.SoLuong;
                    }
                }

                order.TrangThai = "DaThanhToan";

                // Xóa cart
                var cart = _db.GioHangs
                    .Where(g =>
                        g.UserId == order.UserId
                    )
                    .ToList();

                if (cart.Any())
                {
                    _db.GioHangs.RemoveRange(cart);
                }

                _db.SaveChanges();

                return Json(new
                {
                    RspCode = "00",
                    Message = "Confirm Success"
                });
            }

            // =================================================
            // THANH TOÁN THẤT BẠI
            // =================================================

            order.TrangThai =
                "ThanhToanThatBai";

            _db.SaveChanges();

            return Json(new
            {
                RspCode = "00",
                Message = "Confirm Success"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "VNPay IPN Error: " +
                ex.Message
            );

            return Json(new
            {
                RspCode = "99",
                Message = "Unknown error"
            });
        }
    }

    // =========================================================
    // MOMO
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayOnlineWithMomo()
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return RedirectToAction(
                "Login",
                "Account"
            );
        }

        var cart = _db.GioHangs
            .Include(g => g.SanPhamVariant)
            .Where(g => g.UserId == userId.Value)
            .ToList();

        if (!cart.Any())
        {
            return RedirectToAction(
                "Index",
                "Cart"
            );
        }

        decimal total = cart.Sum(
            x => x.SoLuong *
                 x.SanPhamVariant.Gia
        );

        if (total <= 0)
        {
            TempData["PaymentError"] =
                "Số tiền thanh toán không hợp lệ.";

            return RedirectToAction("Index");
        }

        // =====================================================
        // TẠO ORDER
        // =====================================================

        var order = new DonHang
        {
            UserId = userId.Value,
            NgayDat = DateTime.Now,
            NgayTao = DateTime.Now,
            TongTien = total,
            TrangThai = "ChoThanhToan"
        };

        _db.DonHangs.Add(order);

        _db.SaveChanges();

        // =====================================================
        // CHI TIẾT ORDER
        // =====================================================

        foreach (var item in cart)
        {
            _db.ChiTietDonHangs.Add(
                new ChiTietDonHang
                {
                    DonHangId = order.DonHangId,
                    SanPhamVariantId =
                        item.SanPhamVariantId,
                    SoLuong = item.SoLuong,
                    Gia = item.SanPhamVariant.Gia
                }
            );
        }

        _db.SaveChanges();

        // =====================================================
        // MOMO
        // =====================================================

        string momoUrl =
            await _momoService.CreatePaymentUrl(
                HttpContext,
                total,
                order.DonHangId
            );

        return Redirect(momoUrl);
    }

    // =========================================================
    // MOMO RETURN
    // =========================================================

    [HttpGet]
    public IActionResult MomoReturn(
        string partnerCode,
        string orderId,
        string requestId,
        string amount,
        string orderInfo,
        string orderType,
        string transId,
        string resultCode,
        string message,
        string payType,
        string responseTime,
        string extraData,
        string signature)
    {
        try
        {
            if (!int.TryParse(
                orderId,
                out int orderIdInt))
            {
                return View("PaymentFail");
            }

            var order = _db.DonHangs
                .Include(o => o.ChiTietDonHangs)
                .ThenInclude(c => c.SanPhamVariant)
                .FirstOrDefault(
                    o => o.DonHangId == orderIdInt
                );

            if (order == null)
            {
                return View("PaymentFail");
            }

            // =================================================
            // ĐÃ THANH TOÁN
            // =================================================

            if (order.TrangThai == "DaThanhToan")
            {
                return RedirectToAction(
                    "Success",
                    new { id = order.DonHangId }
                );
            }

            // =================================================
            // KIỂM TRA SIGNATURE
            // =================================================

            string rawData =
                $"partnerCode={partnerCode}" +
                $"&accessKey={_momoService.AccessKey}" +
                $"&requestId={requestId}" +
                $"&amount={amount}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&orderType={orderType}" +
                $"&transId={transId}" +
                $"&message={message}" +
                $"&localMessage=" +
                $"&responseTime={responseTime}" +
                $"&errorCode={resultCode}" +
                $"&payType={payType}" +
                $"&extraData={extraData}";

            bool valid =
                _momoService.ValidateSignature(
                    signature,
                    rawData,
                    _momoService.SecretKey
                );

            // =================================================
            // KIỂM TRA AMOUNT
            // =================================================

            if (!decimal.TryParse(
                amount,
                out decimal momoAmount))
            {
                return View("PaymentFail");
            }

            if (momoAmount != order.TongTien)
            {
                order.TrangThai =
                    "ThanhToanThatBai";

                _db.SaveChanges();

                return View("PaymentFail");
            }

            // =================================================
            // THANH TOÁN THÀNH CÔNG
            // =================================================

            if (valid && resultCode == "0")
            {
                // Kiểm tra kho
                foreach (var item in order.ChiTietDonHangs)
                {
                    if (item.SanPhamVariant == null)
                        continue;

                    if (item.SanPhamVariant.SoLuong <
                        item.SoLuong)
                    {
                        order.TrangThai =
                            "ThanhToanThatBai";

                        _db.SaveChanges();

                        return View("PaymentFail");
                    }
                }

                // Trừ kho
                foreach (var item in order.ChiTietDonHangs)
                {
                    if (item.SanPhamVariant != null)
                    {
                        item.SanPhamVariant.SoLuong -=
                            item.SoLuong;
                    }
                }

                order.TrangThai = "DaThanhToan";

                // Xóa cart
                var cart = _db.GioHangs
                    .Where(g =>
                        g.UserId == order.UserId)
                    .ToList();

                if (cart.Any())
                {
                    _db.GioHangs.RemoveRange(cart);
                }

                _db.SaveChanges();

                return RedirectToAction(
                    "Success",
                    new { id = order.DonHangId }
                );
            }

            // =================================================
            // MOMO THẤT BẠI
            // =================================================

            order.TrangThai =
                "ThanhToanThatBai";

            _db.SaveChanges();

            return View("PaymentFail");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "MoMo Return Error: " +
                ex.Message
            );

            return View("PaymentFail");
        }
    }

    // =========================================================
    // SUCCESS
    // =========================================================

    [HttpGet]
    public IActionResult Success(int id)
    {
        var order = _db.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .ThenInclude(c => c.SanPhamVariant)
            .ThenInclude(v => v.SanPham)
            .FirstOrDefault(
                d => d.DonHangId == id
            );

        if (order == null ||
            order.TrangThai != "DaThanhToan")
        {
            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        return View(order);
    }
}