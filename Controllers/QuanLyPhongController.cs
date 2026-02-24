using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.Services;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class QuanLyPhongController : Controller
    {
        private readonly IPhongTroService _phongTroService;

        public QuanLyPhongController(
            IPhongTroService phongTroService)
        {
            _phongTroService = phongTroService;
        }

        // ===========================
        // 1️⃣ DANH SÁCH PHÒNG
        // ===========================
        public async Task<IActionResult> Index(int? nhaId)
        {
            ViewBag.DanhSachNha = await _phongTroService.GetAllNhaAsync();
            ViewBag.SelectedNhaId = nhaId;

            var dsPhong = await _phongTroService.GetPhongByNhaAsync(nhaId);

            return View(dsPhong);
        }

        // ===========================
        // 2️⃣ CREATE – GET
        // ===========================
        [HttpGet]
        public async Task<IActionResult> Create(int? nhaId)
        {
            if (nhaId == null) return RedirectToAction(nameof(Index));

            var data = await _phongTroService.GetCreateDataAsync(nhaId);
            if (data == null) return NotFound();

            ViewBag.DichVuHienThi = data.DichVuHienThi;
            ViewBag.ThongTinNha = data.NhaTro;

            return View(data.Model);
        }

        // ===========================
        // 3️⃣ CREATE – POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhongTro phong)
        {
            if (!ModelState.IsValid)
            {
                var data = await _phongTroService.GetCreateDataAsync(phong.NhaTroId);
                ViewBag.ThongTinNha = data?.NhaTro;
                ViewBag.DichVuHienThi = data?.DichVuHienThi;
                return View(phong);
            }

            await _phongTroService.CreateAsync(phong);

            return RedirectToAction(nameof(Index), new { nhaId = phong.NhaTroId });
        }

        // ===========================
        // 4️⃣ EDIT – GET
        // ===========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _phongTroService.GetEditDataAsync(id);
            if (data == null) return NotFound();

            ViewBag.DichVuCuaNha = data.DichVuCuaNha;
            ViewBag.ThongTinNha = data.NhaTro;
            return View(data.PhongTro);
        }

        // ===========================
        // 5️⃣ EDIT – POST (CHUẨN)
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PhongTro model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var updated = await _phongTroService.UpdateAsync(model);
            if (!updated) return NotFound();

            return RedirectToAction(nameof(Index), new { nhaId = model.NhaTroId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            var result = await _phongTroService.DeleteMultipleAsync(ids);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message, rooms = result.Rooms });
            }

            return Json(new { success = true });
        }

        // ===========================
        // 7️⃣ DETAILS – XEM CHI TIẾT PHÒNG
        // ===========================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var data = await _phongTroService.GetDetailsAsync(id);
            if (data == null) return NotFound();

            ViewBag.TrangThaiPhong = data.TrangThaiPhong;
            ViewBag.DichVuPhong = data.DichVuPhong;

            return View(data.PhongTro);
        }
    }
}
