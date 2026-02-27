using do_an_tot_nghiep.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý danh mục dịch vụ và giá hiện hành.
    /// </summary>
    [ApiController]
    [Route("api/dichvu")]
    public class DichVuApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DichVuApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách dịch vụ (kèm đơn giá hiện tại).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<DichVu>>> GetAll()
        {
            var services = await _context.DichVu.ToListAsync();
            foreach (var service in services)
            {
                var latestPrice = await _context.ServicePriceHistory
                    .Where(p => p.ServiceId == service.Id && p.IsActive)
                    .FirstOrDefaultAsync();
                service.DonGia = latestPrice?.UnitPrice ?? 0;
            }
            return services;
        }

        /// <summary>
        /// Lấy dịch vụ theo id (kèm đơn giá hiện tại).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DichVu>> GetById(int id)
        {
            var dichVu = await _context.DichVu.FindAsync(id);
            if (dichVu == null) return NotFound();
            var latestPrice = await _context.ServicePriceHistory
                .Where(p => p.ServiceId == dichVu.Id && p.IsActive)
                .FirstOrDefaultAsync();
            dichVu.DonGia = latestPrice?.UnitPrice ?? 0;
            return dichVu;
        }

        /// <summary>
        /// Tạo dịch vụ mới (nếu có DonGia sẽ tạo lịch sử giá).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DichVu>> Create(DichVu dichVu)
        {
            _context.DichVu.Add(dichVu);
            await _context.SaveChangesAsync();

            if (dichVu.DonGia > 0)
            {
                var price = new ServicePriceHistory
                {
                    ServiceId = dichVu.Id,
                    UnitPrice = dichVu.DonGia,
                    EffectiveFrom = DateTime.Today,
                    IsActive = true
                };
                _context.ServicePriceHistory.Add(price);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetById), new { id = dichVu.Id }, dichVu);
        }

        /// <summary>
        /// Cập nhật dịch vụ và cập nhật lịch sử giá nếu đổi đơn giá.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, DichVu dichVu)
        {
            if (id != dichVu.Id) return BadRequest();

            var entity = await _context.DichVu.FindAsync(id);
            if (entity == null) return NotFound();

            entity.Ten = dichVu.Ten;
            entity.LoaiDichVu = dichVu.LoaiDichVu;
            entity.IsActive = dichVu.IsActive;
            entity.IsDeleted = dichVu.IsDeleted;

            var currentPrice = await _context.ServicePriceHistory
                .Where(p => p.ServiceId == id && p.IsActive)
                .FirstOrDefaultAsync();

            if (currentPrice == null || currentPrice.UnitPrice != dichVu.DonGia)
            {
                var newEffectiveDate = DateTime.Today;

                if (currentPrice != null)
                {
                    currentPrice.EffectiveTo = newEffectiveDate.AddDays(-1);
                    currentPrice.IsActive = false;
                }

                var newPrice = new ServicePriceHistory
                {
                    ServiceId = id,
                    UnitPrice = dichVu.DonGia,
                    EffectiveFrom = newEffectiveDate,
                    IsActive = true
                };
                _context.ServicePriceHistory.Add(newPrice);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa dịch vụ và toàn bộ lịch sử giá.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dichVu = await _context.DichVu.FindAsync(id);
            if (dichVu == null) return NotFound();

            var prices = _context.ServicePriceHistory.Where(p => p.ServiceId == id);
            _context.ServicePriceHistory.RemoveRange(prices);
            _context.DichVu.Remove(dichVu);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
