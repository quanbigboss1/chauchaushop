using ChauChau.Context;
using ChauChau.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;

namespace ChauChau.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SanPhamController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Dashboard hoặc form thêm sản phẩm
        public IActionResult Dashboard()
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

            ViewBag.SanPhams = _context.SanPhams
                .OrderByDescending(p => p.NgayTao)
                .ToList();

            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View();
        }

        // POST: Tạo sản phẩm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SanPham sanPham)
        {

            // Lấy tất cả file đã upload từ form
            var files = Request.Form.Files;

            if (sanPham.Variants != null)
            {
                for (int i = 0; i < sanPham.Variants.Count; i++)
                {
                    var variant = sanPham.Variants.ElementAt(i);

                    // Khởi tạo danh sách HinhAnhs
                    variant.HinhAnhs = new List<HinhAnhSanPhamVariant>();

                    // Lấy các file tương ứng với variant
                    var variantFilePrefix = $"Variants[{i}].HinhAnhs";
                    var variantFiles = files.Where(f => f.Name == variantFilePrefix).ToList();

                    foreach (var file in variantFiles)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            var savePath = Path.Combine(_env.WebRootPath, "img", fileName);

                            using (var stream = new FileStream(savePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            variant.HinhAnhs.Add(new HinhAnhSanPhamVariant
                            {
                                HinhAnh = "/img/" + fileName
                            });
                        }
                    }
                }
            }

            _context.SanPhams.Add(sanPham);
            _context.SaveChanges();

            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Dashboard","Admin");
        }

        // POST: Xóa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var sanPham = _context.SanPhams
                .Where(p => p.SanPhamId == id)
                .FirstOrDefault();

            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
                _context.SaveChanges();
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
            }

            return RedirectToAction("Dashboard", "Admin");
        }
    }
}
