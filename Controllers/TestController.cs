using ChauChau.Models;
using ChauChau.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ChauChau.Context;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class TestController : Controller
{
    private readonly ApplicationDbContext _db;

    public TestController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> PayOnline(
    string HoTen,
    string SoDienThoai,
    string DiaChiChiTiet,
    string PhuongXa,
    string QuanHuyen,
    string TinhThanh)
    {
        // Lấy user
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Lấy giỏ hàng
        var gioHang = await _db.GioHangs
            .Include(g => g.SanPhamVariant)
                .ThenInclude(v => v.SanPham)
            .Where(g => g.UserId == userId)
            .ToListAsync();

        if (!gioHang.Any())
            return Content("Giỏ hàng trống!");

        // Tính tổng tiền
        decimal tongTien = gioHang.Sum(g => g.SoLuong * g.SanPhamVariant.Gia);

        //=== TẠO ĐƠN HÀNG ===
        var donHang = new DonHang
        {
            UserId = userId,
            TongTien = tongTien,
            TrangThai = "Đang xác thực thanh toán",
            NgayDat = DateTime.Now,
            NgayTao = DateTime.Now,

            HoTen = HoTen,
            SoDienThoai = SoDienThoai,
            DiaChiChiTiet = DiaChiChiTiet,
            PhuongXa = PhuongXa,
            QuanHuyen = QuanHuyen,
            TinhThanh = TinhThanh,

            ChiTietDonHangs = gioHang.Select(g => new ChiTietDonHang
            {
                SanPhamVariantId = g.SanPhamVariantId,
                SoLuong = g.SoLuong,
                Gia = g.SanPhamVariant.Gia
            }).ToList()
        };

        // Lưu đơn hàng (chưa SaveChanges để còn xử lý kho)
        _db.DonHangs.Add(donHang);

        //=== TRỪ TỒN KHO ===
        foreach (var item in donHang.ChiTietDonHangs)
        {
            var variant = await _db.SanPhamVariants
                .Include(v => v.SanPham)
                .FirstOrDefaultAsync(v => v.SanPhamVariantId == item.SanPhamVariantId);

            if (variant == null)
                return Content("Không tìm thấy sản phẩm!");

            // Kiểm tra tồn kho
            if (variant.SoLuong < item.SoLuong)
            {
                return Content(
                    $"❗ Sản phẩm '{variant.SanPham.TenSanPham}' ({variant.MauSac} - {variant.KichCo}) không đủ tồn kho!"
                );
            }

            // Trừ kho
            variant.SoLuong -= item.SoLuong;

            // Nếu muốn: tự động chuyển trạng thái hết hàng
            // if (variant.SoLuong == 0) variant.TrangThai = "Hết hàng";
        }

        // Xóa giỏ hàng
        _db.GioHangs.RemoveRange(gioHang);

        // Lưu DB
        await _db.SaveChangesAsync();

        // --- Tạo URL thanh toán VNPay ---
        var vnpUrl = VNPayConfig.vnp_Url;
        var vnp_ReturnUrl = VNPayConfig.vnp_ReturnUrl;
        var vnp_TmnCode = VNPayConfig.vnp_TmnCode;
        var vnp_HashSecret = VNPayConfig.vnp_HashSecret;

        var vnp_Amount = (int)(donHang.TongTien * 100); // VNPay tính bằng đồng *100
        var vnp_Command = "pay";
        var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        var vnp_CurrCode = "VND";
        var vnp_TxnRef = donHang.DonHangId.ToString();
        var vnp_IpAddr = HttpContext.Connection.RemoteIpAddress?.ToString();
        var vnp_Locale = "vn";

        var queryString = new Dictionary<string, string>
{
    { "vnp_Version", "2.1.0" },
    { "vnp_Command", vnp_Command },
    { "vnp_TmnCode", vnp_TmnCode },
    { "vnp_Amount", vnp_Amount.ToString() },
    { "vnp_CurrCode", vnp_CurrCode },
    { "vnp_TxnRef", vnp_TxnRef },
    { "vnp_OrderInfo", Uri.EscapeDataString($"Thanh toán đơn hàng {donHang.DonHangId}") }, // <- encode
    { "vnp_OrderType", "other" },
    { "vnp_Locale", vnp_Locale },
    { "vnp_ReturnUrl", vnp_ReturnUrl },
    { "vnp_IpAddr", vnp_IpAddr },
    { "vnp_CreateDate", vnp_CreateDate }
};


        // Sắp xếp theo key alphabet
        var sortedQuery = queryString.OrderBy(q => q.Key).ToList();
        var hashData = string.Join("&", sortedQuery.Select(q => $"{q.Key}={Uri.EscapeDataString(q.Value)}"));

        var vnp_SecureHash = HmacSHA512(vnp_HashSecret, hashData);

        var paymentUrl = vnpUrl + "?" + hashData + "&vnp_SecureHash=" + vnp_SecureHash;

        return Redirect(paymentUrl);
    }

    // Callback VNPay
    public async Task<IActionResult> PaymentConfirm()
    {
        var vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();
        var vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();

        var donHangId = int.Parse(vnp_TxnRef);
        var donHang = await _db.DonHangs
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPhamVariant)
                    .ThenInclude(v => v.SanPham) // <- quan trọng
            .FirstOrDefaultAsync(d => d.DonHangId == donHangId);

        if (donHang == null)
            return NotFound();

        if (vnp_ResponseCode == "00")
        {
            donHang.TrangThai = "DaThanhToan";
            await _db.SaveChangesAsync();
            ViewBag.Message = "Thanh toán VNPay thành công!";
        }
        else
        {
            donHang.TrangThai = "ThatBai";
            await _db.SaveChangesAsync();
            ViewBag.Message = "Thanh toán VNPay thất bại!";
        }

        return View(donHang);
    }

    private string HmacSHA512(string key, string data)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key)))
        {
            var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return string.Concat(hashValue.Select(b => b.ToString("x2")));
        }
    }
}
