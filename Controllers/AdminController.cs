using ChauChau.Context;
using ChauChau.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChauChau.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
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

            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.DanhMucs.CountAsync();
            var totalSanPham = await _context.SanPhams.CountAsync();
            var totalOrders = await _context.DonHangs.CountAsync();
            var users =  _context.Users.ToList();
            var today = DateTime.Today;
            var revenueToday = await _context.DonHangs
       .Where(d => d.NgayTao.Date == today && d.TrangThai == "DaThanhToan")
       .SumAsync(d => (decimal?)d.TongTien) ?? 0;
            var totalOrdersToday = await _context.DonHangs
    .Where(d => d.NgayTao.Date == today)
    .CountAsync();
            ViewBag.TotalOrdersToday = totalOrdersToday;

            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var revenueMonth = await _context.DonHangs
                .Where(d => d.NgayTao >= firstDayOfMonth && d.TrangThai == "DaThanhToan")
                .SumAsync(d => (decimal?)d.TongTien) ?? 0;

            var totalStock = await _context.SanPhamVariants.SumAsync(v => (int?)v.SoLuong) ?? 0;

            // Dữ liệu biểu đồ: doanh thu theo ngày
            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(today.Year, today.Month)).ToList();
            var revenuePerDay = new List<decimal>();

            foreach (var day in daysInMonth)
            {
                var date = new DateTime(today.Year, today.Month, day);

                var revenue = await _context.DonHangs
                    .Where(d => d.NgayTao.Date == date && d.TrangThai == "DaThanhToan")
                    .SumAsync(d => (decimal?)d.TongTien) ?? 0;

                revenuePerDay.Add(revenue);
            }
            var products = _context.SanPhams
        .Include(p => p.Variants)
        .Include(p => p.DanhMuc)
        .ToList(); // chuyển sang List, không dùng anonymous type

            ViewBag.SanPhams = products;
            // Lấy danh sách đơn hàng thanh toán thành công hôm nay
            var ordersToday = _context.DonHangs
                .Where(d => d.TrangThai == "DaThanhToan" && d.NgayTao >= today && d.NgayTao < today.AddDays(1))
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPhamVariant)
                    .ThenInclude(v => v.SanPham)
                .Include(d=>d.User)
                .ToList();

            ViewBag.OrdersToday = ordersToday;
            // Đơn hàng thanh toán thành công trong tháng
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            var ordersMonth = _context.DonHangs
                .Where(d => d.TrangThai == "DaThanhToan" && d.NgayTao >= startOfMonth && d.NgayTao < endOfMonth)
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(ct => ct.SanPhamVariant)
                .ThenInclude(v => v.SanPham)
                .Include(d => d.User)
                .ToList();
            ViewBag.OrdersMonth = ordersMonth;
            ViewBag.Users = users;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalSanPham = totalSanPham;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.RevenueToday = revenueToday;
            ViewBag.RevenueMonth = revenueMonth;
            ViewBag.TotalStock = totalStock;
            ViewBag.DaysInMonth = daysInMonth;
            ViewBag.RevenuePerDay = revenuePerDay;
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            ViewBag.Products = _context.SanPhams.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSanPham(
    string TenSanPham,
    string? MoTa,
    int DanhMucId,
    string MauSac,
    string KichCo,
    decimal Gia,
    int SoLuong,
    IFormFile[] HinhAnh)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra, vui lòng kiểm tra lại thông tin!";
                return RedirectToAction("Dashboard");
            }

            // 1. Thêm sản phẩm
            var sanPham = new SanPham
            {
                TenSanPham = TenSanPham,
                MoTa = MoTa,
                DanhMucId = DanhMucId,
                NgayTao = DateTime.Now
            };
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync(); // Lưu để có SanPhamId

            // 2. Thêm Variant (màu sắc + kích cỡ)
            var variant = new SanPhamVariant
            {
                SanPhamId = sanPham.SanPhamId,
                MauSac = MauSac,
                KichCo = KichCo,
                Gia = Gia,
                SoLuong = SoLuong
            };
            _context.SanPhamVariants.Add(variant);
            await _context.SaveChangesAsync(); // Lưu để có SanPhamVariantId

            // 3. Lưu hình ảnh
            foreach (var file in HinhAnh)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var hinhAnh = new HinhAnhSanPhamVariant
                    {
                        SanPhamVariantId = variant.SanPhamVariantId,
                        HinhAnh = "/img/" + fileName,
                        NgayTao = DateTime.Now
                    };

                    _context.HinhAnhSanPhamVariants.Add(hinhAnh);
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm sản phẩm mới thành công!";
            return RedirectToAction("Dashboard");
        }


    }
}
