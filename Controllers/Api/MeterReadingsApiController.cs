using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quan ly chi so dien nuoc theo phong/thang.
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
        /// Danh sach chi so (loc theo roomId, monthYear).
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
        /// Lay chi so theo id.
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<ActionResult<MeterReading>> GetById(long id)
        {
            var reading = await _context.MeterReadings.FindAsync(id);
            if (reading == null) return NotFound();
            return reading;
        }

        /// <summary>
        /// Tao chi so moi. Moi phong moi thang chi co 1 ban ghi.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MeterReading>> Create(MeterReading reading)
        {
            var exists = await _context.MeterReadings
                .AnyAsync(x => x.RoomId == reading.RoomId && x.MonthYear == reading.MonthYear);
            if (exists)
            {
                return Conflict("Da ton tai chi so cho phong va thang nay.");
            }

            var previous = await _context.MeterReadings
                .Where(x => x.RoomId == reading.RoomId)
                .OrderByDescending(x => x.ReadingDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (previous != null)
            {
                if (reading.OldElectricityIndex == 0)
                {
                    reading.OldElectricityIndex = previous.NewElectricityIndex;
                }

                if (reading.OldWaterIndex == 0)
                {
                    reading.OldWaterIndex = previous.NewWaterIndex;
                }

                reading.PreviousReadingId = previous.Id;
            }

            if (reading.ReadingDate == default)
            {
                reading.ReadingDate = DateTime.UtcNow;
            }

            _context.MeterReadings.Add(reading);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = reading.Id }, reading);
        }

        /// <summary>
        /// Cap nhat chi so.
        /// </summary>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, MeterReading reading)
        {
            if (id != reading.Id) return BadRequest();

            _context.Entry(reading).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xoa chi so.
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var reading = await _context.MeterReadings.FindAsync(id);
            if (reading == null) return NotFound();

            _context.MeterReadings.Remove(reading);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
