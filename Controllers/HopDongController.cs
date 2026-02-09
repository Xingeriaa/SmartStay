using System;
using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.Services;
using do_an_tot_nghiep.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace do_an_tot_nghiep.Controllers
{
    public class HopDongController : Controller
    {
        private readonly IHopDongService _hopDongService;

        public HopDongController(IHopDongService hopDongService)
        {
            _hopDongService = hopDongService;
        }

        public async Task<IActionResult> Index()
        {
            var hopDongs = await _hopDongService.GetAllAsync();

            return View(hopDongs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var hopDong = await _hopDongService.GetDetailsAsync(id);

            if (hopDong == null) return NotFound();
            _hopDongService.MapLiquidationToViewFields(hopDong);
            return View(hopDong);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await _hopDongService.BuildCreateFormAsync();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HopDongFormViewModel vm)
        {
            var errors = await _hopDongService.ValidateCreateAsync(vm);
            foreach (var (key, message) in errors)
            {
                ModelState.AddModelError(key, message);
            }

            if (!ModelState.IsValid)
            {
                await _hopDongService.PopulateCreateListsAsync(vm);
                return View(vm);
            }

            await _hopDongService.CreateAsync(vm);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hopDong = await _hopDongService.GetForEditAsync(id);

            if (hopDong == null) return NotFound();
            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HopDong model)
        {
            if (id != model.Id) return NotFound();

            if (!model.NgayKetThuc.HasValue)
            {
                ModelState.AddModelError(nameof(model.NgayKetThuc), "Vui lòng nhập ngày kết thúc.");
            }
            else if (model.NgayKetThuc <= model.NgayBatDau)
            {
                ModelState.AddModelError(nameof(model.NgayKetThuc), "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updated = await _hopDongService.UpdateAsync(id, model);
            if (!updated) return NotFound();

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Extend(int id)
        {
            var hopDong = await _hopDongService.GetForExtendAsync(id);

            if (hopDong == null) return NotFound();
            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Extend(int id, DateTime? ngayKetThucMoi)
        {
            var result = await _hopDongService.ExtendAsync(id, ngayKetThucMoi);
            if (result.HopDong == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                ModelState.AddModelError("ngayKetThucMoi", result.ErrorMessage);
                return View(result.HopDong);
            }

            return RedirectToAction(nameof(Details), new { id = result.HopDong.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Terminate(int id)
        {
            var hopDong = await _hopDongService.GetForTerminateAsync(id);

            if (hopDong == null) return NotFound();

            if (hopDong.NgayThanhLy == null)
            {
                hopDong.NgayThanhLy = DateTime.Today;
            }

            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Terminate(int id, HopDong model)
        {
            var staffId = await _hopDongService.ResolveUserIdAsync(HttpContext.Session.GetString("UserName"));
            var result = await _hopDongService.TerminateAsync(id, model, staffId);
            if (!result.Success) return NotFound();

            return RedirectToAction(nameof(Details), new { id = result.HopDongId });
        }
    }
}
