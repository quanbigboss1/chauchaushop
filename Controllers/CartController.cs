using Microsoft.AspNetCore.Mvc;
using ChauChau.Context;
using ChauChau.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;

public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;

    // Hằng số cho số lượng tối thiểu/tối đa
    private const int MIN_QUANTITY = 1;
    private const int MAX_QUANTITY = 100; // Có thể điều chỉnh hoặc lấy từ tồn kho

    public CartController(ApplicationDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    // Hàm tiện ích để lấy UserId
    private int? GetUserId()
    {
        var userIdClaim = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        return null;
    }

    // ================= HIỆN GIỎ HÀNG (View) =================
    public IActionResult Index()
    {

        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cartItems = _db.GioHangs
            .Include(g => g.SanPhamVariant)
            .ThenInclude(v => v.SanPham)
            .Where(g => g.UserId == userId.Value)
            .ToList();
        
        return View(cartItems);
    }

    // ================= THÊM SẢN PHẨM VÀO GIỎ HÀNG (AJAX) =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int VariantId, int Quantity)
    {
        try
        {
            if (VariantId <= 0 || Quantity <= 0)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });

            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm!" });
            }

            var variant = _db.SanPhamVariants.Find(VariantId);
            if (variant == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });

            // Lấy item hiện tại (đã có) hoặc tạo mới
            var existing = _db.GioHangs.FirstOrDefault(g => g.UserId == userId.Value && g.SanPhamVariantId == VariantId);

            int newTotalQuantity = (existing?.SoLuong ?? 0) + Quantity;

            if (newTotalQuantity > variant.SoLuong)
                return Json(new { success = false, message = "Số lượng đã vượt quá tồn kho!" });


            if (existing != null)
            {
                existing.SoLuong = newTotalQuantity;
                _db.GioHangs.Update(existing);
            }
            else
            {
                var cartItem = new GioHang
                {
                    UserId = userId.Value,
                    SanPhamVariantId = VariantId,
                    SoLuong = Quantity,
                    NgayTao = DateTime.Now
                };
                _db.GioHangs.Add(cartItem);
            }

            _db.SaveChanges();

            int cartCount = _db.GioHangs.Where(g => g.UserId == userId.Value).Sum(g => g.SoLuong);

            return Json(new { success = true, cartCount });
        }
        catch (Exception ex)
        {
            // TODO: log ex
            return Json(new { success = false, message = "Lỗi server khi thêm sản phẩm!" });
        }
    }

    // ================= TĂNG SỐ LƯỢNG (AJAX) =================
    [HttpPost]
    public IActionResult Increase(int id) // id ở đây là GioHangId
    {
        var userId = GetUserId();
        if (userId == null) return Json(new { success = false, message = "Chưa đăng nhập!" });

        var cartItem = _db.GioHangs
            .Include(g => g.SanPhamVariant)
            .FirstOrDefault(g => g.GioHangId == id && g.UserId == userId.Value);

        if (cartItem == null) return Json(new { success = false, message = "Không tìm thấy mục giỏ hàng." });

        int newQuantity = cartItem.SoLuong + 1;

        if (newQuantity > cartItem.SanPhamVariant.SoLuong)
        {
            return Json(new { success = false, message = "Số lượng vượt quá tồn kho." });
        }

        if (newQuantity > MAX_QUANTITY)
        {
            return Json(new { success = false, message = $"Số lượng tối đa là {MAX_QUANTITY}" });
        }

        cartItem.SoLuong = newQuantity;
        _db.GioHangs.Update(cartItem);
        _db.SaveChanges();

        decimal newTotal = cartItem.SoLuong * cartItem.SanPhamVariant.Gia;

        // Trả về JSON để cập nhật giao diện
        return Json(new { success = true, quantity = newQuantity, total = newTotal });
    }

    // ================= GIẢM SỐ LƯỢNG (AJAX) =================
    [HttpPost]
    public IActionResult Decrease(int id) // id ở đây là GioHangId
    {
        var userId = GetUserId();
        if (userId == null) return Json(new { success = false, message = "Chưa đăng nhập!" });

        var cartItem = _db.GioHangs
            .Include(g => g.SanPhamVariant)
            .FirstOrDefault(g => g.GioHangId == id && g.UserId == userId.Value);

        if (cartItem == null) return Json(new { success = false, message = "Không tìm thấy mục giỏ hàng." });

        int newQuantity = cartItem.SoLuong - 1;

        if (newQuantity < MIN_QUANTITY)
        {
            // Nếu giảm xuống dưới 1, xóa khỏi giỏ hàng
            _db.GioHangs.Remove(cartItem);
            _db.SaveChanges();

            // Trả về tín hiệu xóa khỏi giao diện
            return Json(new { success = true, remove = true });
        }

        // Giảm số lượng
        cartItem.SoLuong = newQuantity;
        _db.GioHangs.Update(cartItem);
        _db.SaveChanges();

        decimal newTotal = cartItem.SoLuong * cartItem.SanPhamVariant.Gia;

        // Trả về JSON để cập nhật giao diện
        return Json(new { success = true, quantity = newQuantity, total = newTotal });
    }

    // ================= XÓA SẢN PHẨM (AJAX) =================
    [HttpPost]
    public IActionResult Delete(int id) // id ở đây là GioHangId
    {
        var userId = GetUserId();
        if (userId == null) return Json(new { success = false, message = "Chưa đăng nhập!" });

        var cartItem = _db.GioHangs
            .FirstOrDefault(g => g.GioHangId == id && g.UserId == userId.Value);

        if (cartItem == null)
        {
            // Vẫn trả về success=true nếu mục không tồn tại để giao diện xóa đi không bị lỗi
            return Json(new { success = true, message = "Mục đã được xóa." });
        }

        _db.GioHangs.Remove(cartItem);
        _db.SaveChanges();

        return Json(new { success = true });
    }
    // Action lấy danh sách đơn hàng của user
    public async Task<IActionResult> MyOrders()
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Account"); // nếu chưa đăng nhập

        var donHangs = await _db.DonHangs
            .Where(d => d.UserId == userId.Value && d.TrangThai == "DaThanhToan")

            // Gộp Include để tránh lặp --> CHUẨN nhất
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPhamVariant)
                    .ThenInclude(v => v.SanPham)

            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPhamVariant)
                    .ThenInclude(v => v.HinhAnhs)

            .OrderByDescending(d => d.NgayDat)
            .ToListAsync();

        return View(donHangs);
    }

}