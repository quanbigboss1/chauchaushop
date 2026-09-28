using ChauChau.Context;
using ChauChau.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChauChau.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email và mật khẩu không được để trống!";
                return View();
            }

            var user = await Task.Run(() => _context.Users.FirstOrDefault(u => u.Email == email));

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.MatKhau))
            {
                // Tạo claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.HoTen),
            new Claim(ClaimTypes.Email, user.Email),
                  new Claim(ClaimTypes.Role, user.Role)
            // Bạn có thể thêm Role: new Claim(ClaimTypes.Role, "Admin")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // lưu cookie lâu dài
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );
                // thông báo thành công
                TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {user.HoTen}";
                return RedirectToAction("TrangChu", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            return View();
        }
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string hoTen, string email, string password, string soDienThoai)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email và mật khẩu không được để trống!";
                return View();
            }

            // Kiểm tra email đã tồn tại chưa
            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email đã được đăng ký!";
                return View();
            }

            var user = new User
            {
                HoTen = hoTen,
                Email = email,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(password), // hash password
                SoDienThoai = soDienThoai,
                Role = "Khách Hàng",
                NgayTao = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        [HttpPost] // phải là POST
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Profile()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string HoTen, string SoDienThoai)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.HoTen = HoTen;
                user.SoDienThoai = SoDienThoai;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(OldPassword, user.MatKhau))
                {
                    TempData["ErrorMessage"] = "Mật khẩu cũ không đúng!";
                    return RedirectToAction("Profile");
                }
                if (NewPassword != ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "Xác nhận mật khẩu không khớp!";
                    return RedirectToAction("Profile");
                }

                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            }

            return RedirectToAction("Profile");
        }
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard", "Admin"); // hoặc trang bạn muốn
        }

    }
}
