using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Xử lý yêu cầu hỗ trợ (ticket) của cư dân.
    /// </summary>
    [ApiController]
    [Route("api/tickets")]
    public class TicketsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách ticket (lọc theo status, createdBy, assignedTo).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Ticket>>> GetAll([FromQuery] string? status, [FromQuery] int? createdBy, [FromQuery] int? assignedTo)
        {
            var query = _context.Tickets.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            if (createdBy.HasValue)
            {
                query = query.Where(x => x.CreatedBy == createdBy.Value);
            }

            if (assignedTo.HasValue)
            {
                query = query.Where(x => x.AssignedTo == assignedTo.Value);
            }

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        /// <summary>
        /// Lấy ticket theo id (kèm ảnh).
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<ActionResult<Ticket>> GetById(long id)
        {
            var ticket = await _context.Tickets
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ticket == null) return NotFound();
            return ticket;
        }

        /// <summary>
        /// Tạo ticket mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Ticket>> Create(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        /// <summary>
        /// Cập nhật ticket.
        /// </summary>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, Ticket ticket)
        {
            if (id != ticket.Id) return BadRequest();

            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Gán người xử lý ticket.
        /// </summary>
        [HttpPost("{id:long}/assign")]
        public async Task<IActionResult> Assign(long id, AssignTicketRequest request)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.AssignedTo = request.AssignedTo;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Cập nhật trạng thái ticket.
        /// </summary>
        [HttpPost("{id:long}/status")]
        public async Task<IActionResult> UpdateStatus(long id, UpdateTicketStatusRequest request)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.Status = request.Status;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa ticket.
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var images = _context.TicketImages.Where(x => x.TicketId == id);
            _context.TicketImages.RemoveRange(images);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Thêm ảnh cho ticket.
        /// </summary>
        [HttpPost("{id:long}/images")]
        public async Task<ActionResult<TicketImage>> AddImage(long id, TicketImage image)
        {
            if (id != image.TicketId) return BadRequest();

            _context.TicketImages.Add(image);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id }, image);
        }

        public class AssignTicketRequest
        {
            public int? AssignedTo { get; set; }
        }

        public class UpdateTicketStatusRequest
        {
            public string Status { get; set; } = "Open";
        }
    }
}
