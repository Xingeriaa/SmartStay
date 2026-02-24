using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.Services;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class DichVuController : Controller
    {
        private readonly IDichVuService _dichVuService;

        public DichVuController(IDichVuService dichVuService)
        {
            _dichVuService = dichVuService;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var services = await _dichVuService.GetServicesAsync(searchString);
            return View(services);
        }

        // 2. Thêm mới
        public IActionResult Create()
        {
            ViewBag.LoaiDichVuList = _dichVuService.GetLoaiDichVuList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Chống tấn công giả mạo yêu cầu
        public async Task<IActionResult> Create(DichVu model)
        {
            if (ModelState.IsValid)
            {
                await _dichVuService.CreateAsync(model);

                return RedirectToAction(nameof(Index)); // Dùng nameof để tránh viết sai chuỗi
            }
            ViewBag.LoaiDichVuList = _dichVuService.GetLoaiDichVuList();
            return View(model);
        }

        // 3. Chỉnh sửa (Nếu bạn đã xóa nút Edit ở View thì có thể xóa code này, hoặc giữ lại làm API)
        public async Task<IActionResult> Edit(int id)
        {
            var dichVu = await _dichVuService.GetForEditAsync(id);
            if (dichVu == null) return NotFound();

            ViewBag.LoaiDichVuList = _dichVuService.GetLoaiDichVuList();
            return View(dichVu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DichVu model)
        {
            if (ModelState.IsValid)
            {
                var updated = await _dichVuService.UpdateAsync(model);
                if (!updated) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiDichVuList = _dichVuService.GetLoaiDichVuList();
            return View(model);
        }

        // 4. Xóa đơn lẻ (Giữ lại để dự phòng)
        public async Task<IActionResult> Delete(int id)
        {
            await _dichVuService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // 5. Xóa nhiều mục (AJAX) - Đã tối ưu
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> ids)
        {
            var result = await _dichVuService.DeleteSelectedAsync(ids);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true });
        }
    }
}
