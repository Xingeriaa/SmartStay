using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý phòng trọ.
    /// </summary>
    [ApiController]
    [Route("api/phongtro")]
    public class PhongTroApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PhongTroApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách phòng.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PhongTro>>> GetAll()
        {
            return await _context.PhongTros.ToListAsync();
        }

        /// <summary>
        /// Lấy phòng theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PhongTro>> GetById(int id)
        {
            var phong = await _context.PhongTros.FindAsync(id);
            if (phong == null) return NotFound();
            return phong;
        }

        /// <summary>
        /// Tạo phòng mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PhongTro>> Create(PhongTro phong)
        {
            if (string.IsNullOrWhiteSpace(phong.DoiTuong))
            {
                phong.DoiTuong = "Standard";
            }
            _context.PhongTros.Add(phong);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = phong.Id }, phong);
        }

        /// <summary>
        /// Cập nhật phòng.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, PhongTro phong)
        {
            if (id != phong.Id) return BadRequest();

            if (string.IsNullOrWhiteSpace(phong.DoiTuong))
            {
                phong.DoiTuong = "Standard";
            }
            _context.Entry(phong).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa phòng.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var phong = await _context.PhongTros.FindAsync(id);
            if (phong == null) return NotFound();

            _context.PhongTros.Remove(phong);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
