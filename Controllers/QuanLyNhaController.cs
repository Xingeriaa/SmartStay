using Microsoft.AspNetCore.Mvc;
using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace do_an_tot_nghiep.Controllers
{
    public class QuanLyNhaController : Controller
    {
        private readonly IQuanLyNhaService _quanLyNhaService;

        public QuanLyNhaController(IQuanLyNhaService quanLyNhaService)
        {
            _quanLyNhaService = quanLyNhaService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _quanLyNhaService.GetIndexDataAsync();
            ViewBag.AllDichVus = data.AllDichVus;
            return View(data.NhaTros);
        }

        // GET: QuanLyNha/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Nạp danh sách dịch vụ để hiển thị trong bảng chọn
            ViewBag.ListDichVu = await _quanLyNhaService.GetDichVuListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhaTro model, List<string> DichVuSelect)
        {
            // Gỡ bỏ kiểm tra lỗi của trường này nếu nó vẫn báo lỗi trong ModelState
            ModelState.Remove("DichVuSelect");
            ModelState.Remove("DanhSachDichVu");

            if (ModelState.IsValid)
            {
                await _quanLyNhaService.CreateAsync(model, DichVuSelect);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ListDichVu = await _quanLyNhaService.GetDichVuListAsync();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> ids)
        {
            var result = await _quanLyNhaService.DeleteSelectedAsync(ids);
            if (result.Success)
            {
                return Json(new { success = true, message = result.Message });
            }

            return Json(new { success = false, message = result.Message });
        }
    }
}
