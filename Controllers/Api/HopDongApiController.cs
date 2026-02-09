using do_an_tot_nghiep.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý hợp đồng thuê phòng.
    /// </summary>
    [ApiController]
    [Route("api/hopdong")]
    public class HopDongApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HopDongApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách hợp đồng.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<HopDong>>> GetAll()
        {
            return await _context.HopDongs.ToListAsync();
        }

        /// <summary>
        /// Lấy hợp đồng theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<HopDong>> GetById(int id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null) return NotFound();
            return hopDong;
        }

        /// <summary>
        /// Tạo hợp đồng mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<HopDong>> Create(HopDong hopDong)
        {
            if (string.IsNullOrWhiteSpace(hopDong.ContractCode))
            {
                hopDong.ContractCode = $"HD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }

            if (hopDong.SignatureDate == default)
            {
                hopDong.SignatureDate = DateTime.Today;
            }

            if (string.IsNullOrWhiteSpace(hopDong.DepositStatus))
            {
                hopDong.DepositStatus = "Available";
            }

            if (hopDong.PaymentCycle <= 0)
            {
                hopDong.PaymentCycle = 1;
            }

            _context.HopDongs.Add(hopDong);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = hopDong.Id }, hopDong);
        }

        /// <summary>
        /// Cập nhật hợp đồng.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, HopDong hopDong)
        {
            if (id != hopDong.Id) return BadRequest();

            _context.Entry(hopDong).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa hợp đồng.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null) return NotFound();

            _context.HopDongs.Remove(hopDong);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
