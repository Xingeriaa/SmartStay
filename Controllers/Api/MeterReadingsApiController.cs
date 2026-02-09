using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý chỉ số điện nước theo phòng/tháng.
    /// </summary>
    [ApiController]
    [Route("api/meter-readings")]
    public class MeterReadingsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeterReadingsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách chỉ số (lọc theo roomId, monthYear).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<MeterReading>>> GetAll([FromQuery] int? roomId, [FromQuery] string? monthYear)
        {
            var query = _context.MeterReadings.AsNoTracking();
            if (roomId.HasValue)
            {
                query = query.Where(x => x.RoomId == roomId.Value);
            }
            if (!string.IsNullOrWhiteSpace(monthYear))
            {
                query = query.Where(x => x.MonthYear == monthYear);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Lấy chỉ số theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MeterReading>> GetById(int id)
        {
            var reading = await _context.MeterReadings.FindAsync(id);
            if (reading == null) return NotFound();
            return reading;
        }

        /// <summary>
        /// Tạo chỉ số mới. Mỗi phòng mỗi tháng chỉ có 1 bản ghi.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MeterReading>> Create(MeterReading reading)
        {
            var exists = await _context.MeterReadings
                .AnyAsync(x => x.RoomId == reading.RoomId && x.MonthYear == reading.MonthYear);
            if (exists)
            {
                return Conflict("Đã tồn tại chỉ số cho phòng và tháng này.");
            }

            _context.MeterReadings.Add(reading);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = reading.Id }, reading);
        }

        /// <summary>
        /// Cập nhật chỉ số.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, MeterReading reading)
        {
            if (id != reading.Id) return BadRequest();

            _context.Entry(reading).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa chỉ số.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reading = await _context.MeterReadings.FindAsync(id);
            if (reading == null) return NotFound();

            _context.MeterReadings.Remove(reading);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
