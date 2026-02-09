using System.Diagnostics;
using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace do_an_tot_nghiep.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // TRANG GIỚI THIỆU
        public IActionResult Landing()
        {
            return View();
        }

        // DASHBOARD - FORM QUẢN LÝ NHÀ TRỌ
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Landing));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

    }
}
