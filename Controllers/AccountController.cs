using do_an_tot_nghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace do_an_tot_nghiep.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // ════════════════════════════════════════════════════════════════
        // ĐĂNG KÝ — chỉ tạo tài khoản người thuê (NguoiThue)
        // Admin / Staff được Admin cấp quyền qua trang quản trị Users
        // ════════════════════════════════════════════════════════════════
        [HttpGet]
        public IActionResult Register()
        {
            // Nếu đã đăng nhập rồi → redirect
            if (HttpContext.Session.GetString("UserName") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string username, string password, string confirmPw,
            string email, string? fullName, string? phone)
        {
            // Client đã validate JS, nhưng server phải validate lại
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin bắt buộc.";
                return View();
            }

            if (password != confirmPw)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            if (password.Length < 8)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 8 ký tự.";
                return View();
            }

            var result = await _accountService.RegisterAsync(
                username.Trim(), password, email.Trim(),
                fullName?.Trim(), phone?.Trim());

            if (!result.Success)
            {
                ViewBag.Error = result.ErrorMessage;
                return View();
            }

            TempData["RegisterSuccess"] = "Tạo tài khoản thành công! Hãy đăng nhập để tiếp tục.";
            return RedirectToAction(nameof(Login));
        }

        // ════════════════════════════════════════════════════════════════
        // ĐĂNG NHẬP
        // ════════════════════════════════════════════════════════════════
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                return View();
            }

            var result = await _accountService.LoginAsync(username.Trim(), password);

            if (!result.Success || result.User == null)
            {
                ViewBag.Error = result.ErrorMessage;
                // Giữ lại username để không phải gõ lại
                if (rememberMe) ViewBag.SavedUsername = username;
                return View();
            }

            var user = result.User;
            var roleName = result.RoleName ?? "NguoiThue";

            // Lưu session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);
            HttpContext.Session.SetString("Role", roleName);

            // Remember me — lưu username vào cookie 30 ngày
            if (rememberMe)
            {
                Response.Cookies.Append("rm_user", user.Username, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // đổi true khi deploy HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });
            }
            else
            {
                Response.Cookies.Delete("rm_user");
            }

            // Điều hướng theo role — DB schema: "Admin" / "Staff" / "Tenant"
            return roleName switch
            {
                "Admin" => RedirectToAction("Index", "Dashboard"),
                "Staff" => RedirectToAction("Index", "Dashboard"),
                "Tenant" => RedirectToAction("Index", "TenantPortal"),
                _ => RedirectToAction("Index", "Dashboard")
            };
        }

        // ════════════════════════════════════════════════════════════════
        // ĐĂNG XUẤT
        // ════════════════════════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("rm_user");
            return RedirectToAction(nameof(Login));
        }

        // GET Logout (cho link thông thường trong navbar)
        [HttpGet]
        public IActionResult LogoutGet()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // ════════════════════════════════════════════════════════════════
        // ACCESS DENIED
        // ════════════════════════════════════════════════════════════════
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
