using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;

namespace do_an_tot_nghiep.Controllers
{
    // Roles.Tenant = "Tenant" (DB chuẩn) — khớp Session["Role"]
    [RequireRole(Roles.Tenant)]
    public class KhachHangMainController : Controller
    {
        // Action này phải trùng tên với file View "trangchinhtimnha.cshtml"
        public IActionResult trangchinhtimnha()
        {
            // Code lấy dữ liệu nhà nếu cần
            // var listNha = _context.NhaTro.ToList();
            // return View(listNha);

            return View();
        }
    }
}
