using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý cư dân/khách thuê.
    /// </summary>
    [ApiController]
    [Route("api/khachthue")]
    public class KhachThueApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KhachThueApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách khách thuê.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<KhachThue>>> GetAll()
        {
            return await _context.KhachThues.ToListAsync();
        }

        /// <summary>
        /// Lấy khách thuê theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<KhachThue>> GetById(int id)
        {
            var khach = await _context.KhachThues.FindAsync(id);
            if (khach == null) return NotFound();
            return khach;
        }

        /// <summary>
        /// Tạo khách thuê mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<KhachThue>> Create(KhachThue khach)
        {
            _context.KhachThues.Add(khach);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = khach.Id }, khach);
        }

        /// <summary>
        /// Cập nhật khách thuê.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, KhachThue khach)
        {
            if (id != khach.Id) return BadRequest();

            _context.Entry(khach).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa khách thuê.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var khach = await _context.KhachThues.FindAsync(id);
            if (khach == null) return NotFound();

            _context.KhachThues.Remove(khach);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
