using do_an_tot_nghiep.Services;
using Microsoft.AspNetCore.Http;
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

        // --- PHẦN ĐĂNG KÝ ---
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string email, string role)
        {
            var result = await _accountService.RegisterAsync(username, password, email, role);
            if (!result.Success)
            {
                ViewBag.Error = result.ErrorMessage;
                return View();
            }

            // Đăng ký xong chuyển qua trang đăng nhập
            return RedirectToAction("Login");
        }

        // --- PHẦN ĐĂNG NHẬP ---
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            var result = await _accountService.LoginAsync(username, password);
            if (!result.Success || result.User == null)
            {
                ViewBag.Error = result.ErrorMessage;
                return View();
            }

            var user = result.User;

            // 1. Lưu Session
            HttpContext.Session.SetString("UserName", user.Username);

            if (!string.IsNullOrEmpty(result.RoleName))
            {
                HttpContext.Session.SetString("Role", result.RoleName);
            }

            // 2. Điều hướng theo Role
            if (result.RoleName == "Admin")
            {
                // Nếu là Chủ trọ -> Vào Controller QuanLyNha
                return RedirectToAction("Index", "QuanLyNha");
            }
            else if (result.RoleName == "NguoiThue")
            {
                // Nếu là Người thuê -> Vào Controller KhachHangMain
                // Lưu ý: Đảm bảo bạn đã tạo KhachHangMainController.cs như hướng dẫn trước
                return RedirectToAction("trangchinhtimnha", "KhachHangMain");
            }

            // Trường hợp dự phòng (Role bị null hoặc sai) -> Về trang chủ mặc định
            return RedirectToAction("Index", "Home");
        }

        // --- ĐĂNG XUẤT ---
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
