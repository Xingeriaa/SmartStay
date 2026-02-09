using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class KhachThueController : Controller
{
    private readonly IKhachThueService _khachThueService;

    public KhachThueController(IKhachThueService khachThueService)
    {
        _khachThueService = khachThueService;
    }

    public async Task<IActionResult> Index()
    {
        var ds = await _khachThueService.GetAllAsync();
        return View(ds);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KhachThue model, IFormFile? fHinhAnh)
    {
        // Loại bỏ kiểm tra bắt buộc cho các trường tùy chọn
        ModelState.Remove("SoDienThoai2");
        ModelState.Remove("SoTienCoc");
        ModelState.Remove("HinhAnh");

        if (ModelState.IsValid)
        {
            await _khachThueService.CreateAsync(model, fHinhAnh);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }
    // XEM CHI TIẾT
    public async Task<IActionResult> Details(int id)
    {
        var item = await _khachThueService.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // SỬA (GET)
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _khachThueService.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, KhachThue model, IFormFile? fHinhAnh)
    {
        if (id != model.Id) return NotFound();
        ModelState.Remove("HinhAnh");

        if (ModelState.IsValid)
        {
            var updated = await _khachThueService.UpdateAsync(id, model, fHinhAnh);
            if (!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // XÓA (Thực hiện xóa trực tiếp)
    public async Task<IActionResult> Delete(int id)
    {
        await _khachThueService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
