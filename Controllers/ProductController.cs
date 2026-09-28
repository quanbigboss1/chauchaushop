using ChauChau.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChauChau.Controllers
{
    [Route("Product")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("AllProduct")]
        public async Task<IActionResult> AllProduct(int? categoryId, decimal? minPrice, decimal? maxPrice)
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

            // Load danh mục cho menu
            ViewBag.Categories = await _context.DanhMucs.ToListAsync();

            // Query sản phẩm
            var products = _context.SanPhams
                .Include(p => p.Variants).ThenInclude(v => v.HinhAnhs)
                .AsQueryable();

            // ===== LỌC DANH MỤC =====
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.DanhMucId == categoryId.Value);
            }

            // ===== LỌC GIÁ =====
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Variants.Any(v => v.Gia >= minPrice));
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Variants.Any(v => v.Gia <= maxPrice));
            }

            // Lưu để dùng lại trong view
            ViewBag.SelectedCategory = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(await products.ToListAsync());
        }


        [Route("Details/{id}/{name?}")] // ? để name là optional
        public async Task<IActionResult> Details(int id, string name = "")
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


            var product = await _context.SanPhams
                .Include(p => p.DanhMuc) // load danh mục
                .Include(p => p.Variants)
                    .ThenInclude(v => v.HinhAnhs) // load hình ảnh variant
                .FirstOrDefaultAsync(p => p.SanPhamId == id);

            if (product == null)
                return NotFound();

            // Tùy chọn: kiểm tra name trong url có trùng với product.TenSanPham không
            if (!string.IsNullOrEmpty(name) && !string.Equals(name, product.TenSanPham, StringComparison.OrdinalIgnoreCase))
            {
                // redirect về url đúng nếu muốn SEO-friendly
                return RedirectToAction("Details", new { id = id, name = product.TenSanPham });
            }

            return View(product);
        }

    }
}
