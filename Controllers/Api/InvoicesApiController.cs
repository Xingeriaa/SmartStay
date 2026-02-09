using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý hóa đơn và chi tiết hóa đơn.
    /// </summary>
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoicesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách hóa đơn (lọc theo contractId, status, monthYear).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Invoice>>> GetAll([FromQuery] int? contractId, [FromQuery] string? status, [FromQuery] string? monthYear)
        {
            var query = _context.Invoices.AsNoTracking();

            if (contractId.HasValue)
            {
                query = query.Where(x => x.ContractId == contractId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(monthYear))
            {
                query = query.Where(x => x.MonthYear == monthYear);
            }

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        /// <summary>
        /// Lấy hóa đơn theo id (kèm chi tiết).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Invoice>> GetById(int id)
        {
            var invoice = await _context.Invoices
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null) return NotFound();
            return invoice;
        }

        /// <summary>
        /// Tạo hóa đơn mới (kèm chi tiết).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Invoice>> Create(CreateInvoiceRequest request)
        {
            var invoice = new Invoice
            {
                InvoiceCode = request.InvoiceCode,
                ContractId = request.ContractId,
                Period = request.Period,
                MonthYear = request.MonthYear,
                TaxAmount = request.TaxAmount,
                DueDate = request.DueDate,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "Unpaid" : request.Status,
                CreatedAt = DateTime.UtcNow
            };

            var details = new List<InvoiceDetail>();
            if (request.Details != null)
            {
                foreach (var item in request.Details)
                {
                    var amount = item.Amount;
                    if (amount <= 0)
                    {
                        amount = item.Quantity * item.UnitPriceSnapshot;
                    }

                    details.Add(new InvoiceDetail
                    {
                        Description = item.Description,
                        Quantity = item.Quantity,
                        UnitPriceSnapshot = item.UnitPriceSnapshot,
                        Amount = amount
                    });
                }
            }

            invoice.SubTotal = details.Sum(x => x.Amount);
            invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            if (details.Count > 0)
            {
                foreach (var detail in details)
                {
                    detail.InvoiceId = invoice.Id;
                }

                _context.InvoiceDetails.AddRange(details);
                await _context.SaveChangesAsync();
                invoice.Details = details;
            }

            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        /// <summary>
        /// Cập nhật trạng thái hóa đơn (Unpaid/Paid/Overdue).
        /// </summary>
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateInvoiceStatusRequest request)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            invoice.Status = request.Status;
            invoice.PaymentDate = request.PaymentDate;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa hóa đơn.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            var details = _context.InvoiceDetails.Where(d => d.InvoiceId == id);
            _context.InvoiceDetails.RemoveRange(details);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class CreateInvoiceRequest
        {
            public string InvoiceCode { get; set; } = string.Empty;
            public int ContractId { get; set; }
            public string Period { get; set; } = string.Empty;
            public string MonthYear { get; set; } = string.Empty;
            public decimal TaxAmount { get; set; }
            public DateTime DueDate { get; set; }
            public string? Status { get; set; }
            public List<InvoiceDetailItem>? Details { get; set; }
        }

        public class InvoiceDetailItem
        {
            public string Description { get; set; } = string.Empty;
            public decimal Quantity { get; set; }
            public decimal UnitPriceSnapshot { get; set; }
            public decimal Amount { get; set; }
        }

        public class UpdateInvoiceStatusRequest
        {
            public string Status { get; set; } = "Unpaid";
            public DateTime? PaymentDate { get; set; }
        }
    }
}
