using do_an_tot_nghiep.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý tòa nhà/nhà trọ.
    /// </summary>
    [ApiController]
    [Route("api/nhatro")]
    public class NhaTroApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NhaTroApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách nhà trọ.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<NhaTro>>> GetAll()
        {
            return await _context.NhaTros.ToListAsync();
        }

        /// <summary>
        /// Lấy nhà trọ theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<NhaTro>> GetById(int id)
        {
            var nha = await _context.NhaTros.FindAsync(id);
            if (nha == null) return NotFound();
            return nha;
        }

        /// <summary>
        /// Tạo nhà trọ mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<NhaTro>> Create(NhaTro nha)
        {
            nha.DiaChiChiTiet = BuildAddress(nha);
            _context.NhaTros.Add(nha);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = nha.Id }, nha);
        }

        /// <summary>
        /// Cập nhật nhà trọ.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, NhaTro nha)
        {
            if (id != nha.Id) return BadRequest();

            nha.DiaChiChiTiet = BuildAddress(nha);
            _context.Entry(nha).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static string BuildAddress(NhaTro model)
        {
            var parts = new List<string>();
            AddPart(parts, model.DiaChiChiTiet);
            AddPart(parts, model.PhuongXa);
            AddPart(parts, model.QuanHuyen);
            AddPart(parts, model.TinhThanh);

            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private static void AddPart(List<string> parts, string? value)
        {
            var trimmed = value?.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                parts.Add(trimmed);
            }
        }

        /// <summary>
        /// Xóa nhà trọ.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var nha = await _context.NhaTros.FindAsync(id);
            if (nha == null) return NotFound();

            _context.NhaTros.Remove(nha);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
