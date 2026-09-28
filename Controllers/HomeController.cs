using System.Diagnostics;
using System.Security.Claims;
using ChauChau.Context;
using ChauChau.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChauChau.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        protected readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context,ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            int cartCount = 0;

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                cartCount = _context.GioHangs
                                    .Where(g => g.UserId == userId)
                                    .Sum(g => g.SoLuong);
            }

            ViewBag.CartCount = cartCount;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Contact()
        {
            int cartCount = 0;

            // Count cart
            if (User.Identity.IsAuthenticated)
            {
                var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userIdValue) && int.TryParse(userIdValue, out int userId))
                {
                    cartCount = _context.GioHangs
                        .Where(g => g.UserId == userId)
                        .Sum(g => g.SoLuong);
                }
            }

            ViewBag.CartCount = cartCount;

            return View();
        }
        public IActionResult About()
        {
            int cartCount = 0;

            // Count cart
            if (User.Identity.IsAuthenticated)
            {
                var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userIdValue) && int.TryParse(userIdValue, out int userId))
                {
                    cartCount = _context.GioHangs
                        .Where(g => g.UserId == userId)
                        .Sum(g => g.SoLuong);
                }
            }

            ViewBag.CartCount = cartCount;

            return View();
        }
        public IActionResult TrangChu()
        {
            int cartCount = 0;

            // Count cart
            if (User.Identity.IsAuthenticated)
            {
                var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userIdValue) && int.TryParse(userIdValue, out int userId))
                {
                    cartCount = _context.GioHangs
                        .Where(g => g.UserId == userId)
                        .Sum(g => g.SoLuong);
                }
            }

            ViewBag.CartCount = cartCount;

            var sanPhamNgauNhien = _context.SanPhams
                .Include(sp=>sp.Variants)
                    .ThenInclude(h=>h.HinhAnhs)
         .AsNoTracking()
         .OrderBy(x => Guid.NewGuid()) 
         .Take(4)
         .ToList();

            ViewBag.SanPhamHot = sanPhamNgauNhien;
            ////
            ////
            ///
            // Lấy TOP 4 sản phẩm bán chạy nhất
            var sanPhamBanChay = _context.ChiTietDonHangs
                .GroupBy(ct => ct.SanPhamVariant.SanPhamId)
                .Select(g => new
                {
                    SanPhamId = g.Key,
                    TongSoLuongBan = g.Sum(x => x.SoLuong)
                })
                .OrderByDescending(x => x.TongSoLuongBan)
                .Take(4)
                .ToList();

            // Lấy danh sách SanPham đầy đủ bao gồm Variant + ảnh
            var topSanPham = _context.SanPhams
                .Where(sp => sanPhamBanChay.Select(x => x.SanPhamId).Contains(sp.SanPhamId))
                .Include(sp => sp.Variants)
                    .ThenInclude(v => v.HinhAnhs)
                .AsNoTracking()
                .ToList();

            ViewBag.SanPhamBanChay = topSanPham;
            return View();
        }
    }
}
