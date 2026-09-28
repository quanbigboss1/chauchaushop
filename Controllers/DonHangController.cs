using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChauChau.Context;
using ChauChau.Models;
using System.Security.Claims;

namespace ChauChau.Controllers
{
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DonHangController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Chi tiết đơn hàng theo ID
        public async Task<IActionResult> Details(int id)
        {
            int cartCount = 0;

            // Count cart
            if (User.Identity.IsAuthenticated)
            {
                var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userIdValue) && int.TryParse(userIdValue, out int userId))
                {
                    cartCount = _db.GioHangs
                        .Where(g => g.UserId == userId)
                        .Sum(g => g.SoLuong);
                }
            }

            ViewBag.CartCount = cartCount;

            var donHang = await _db.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPhamVariant)
                        .ThenInclude(v => v.HinhAnhs) // Load hình ảnh
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPhamVariant)
                        .ThenInclude(v => v.SanPham) // Load tên sản phẩm
                .FirstOrDefaultAsync(d => d.DonHangId == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
    }
}
