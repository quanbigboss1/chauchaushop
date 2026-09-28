using ChauChau.Context;
using ChauChau.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChauChau.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext vào controller
        public DanhMucController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DanhMuc/Index
        public IActionResult Index()
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

            var danhMucs = _context.DanhMucs
                                   .OrderByDescending(dm => dm.NgayTao)
                                   .ToList();
            return View(danhMucs);
        }

        // POST: DanhMuc/Create
        [HttpPost]
        public IActionResult Create(DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp
                bool exists = _context.DanhMucs
                    .Any(dm => dm.TenDanhMuc == danhMuc.TenDanhMuc
                           && dm.MoTa == danhMuc.MoTa);

                if (exists)
                {
                    // Thêm lỗi vào ModelState để hiển thị ra view
                    ModelState.AddModelError("", "Danh mục với tên và mô tả này đã tồn tại.");
                    return RedirectToAction("Dashboard", "Admin");
                }

                _context.DanhMucs.Add(danhMuc);
                _context.SaveChanges();
                return RedirectToAction("Dashboard", "Admin");
            }

            return View(danhMuc);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var danhMuc = _context.DanhMucs.Find(id);
            if (danhMuc != null)
            {
                _context.DanhMucs.Remove(danhMuc);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard", "Admin"); // Hoặc trả về cùng view
        }

    }
}
