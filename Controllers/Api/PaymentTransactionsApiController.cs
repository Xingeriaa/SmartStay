using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quan ly giao dich thanh toan va xac nhan thanh toan.
    /// </summary>
    [ApiController]
    [Route("api/payments")]
    public class PaymentTransactionsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentTransactionsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sach giao dich (loc theo invoiceId).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PaymentTransaction>>> GetAll([FromQuery] int? invoiceId)
        {
            var query = _context.PaymentTransactions.AsNoTracking();
            if (invoiceId.HasValue)
            {
                query = query.Where(x => x.InvoiceId == invoiceId.Value);
            }
            return await query.OrderByDescending(x => x.PaidAt).ToListAsync();
        }

        /// <summary>
        /// Tao giao dich thanh toan.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PaymentTransaction>> Create(PaymentTransaction transaction)
        {
            if (transaction.PaidAt == default)
            {
                transaction.PaidAt = DateTime.UtcNow;
            }

            if (string.IsNullOrWhiteSpace(transaction.TransactionCode))
            {
                transaction.TransactionCode = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = transaction.Id }, transaction);
        }

        /// <summary>
        /// Xac nhan giao dich (ghi nhan nguoi xac nhan).
        /// </summary>
        [HttpPost("{id:long}/confirm")]
        public async Task<IActionResult> Confirm(long id, ConfirmPaymentRequest request)
        {
            var transaction = await _context.PaymentTransactions.FindAsync(id);
            if (transaction == null) return NotFound();

            transaction.ConfirmedBy = request.ConfirmedBy;
            await _context.SaveChangesAsync();

            var invoice = await _context.Invoices.FindAsync(transaction.InvoiceId);
            if (invoice != null)
            {
                invoice.Status = "Paid";
                invoice.PaymentDate = transaction.PaidAt;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Xoa giao dich.
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var transaction = await _context.PaymentTransactions.FindAsync(id);
            if (transaction == null) return NotFound();

            _context.PaymentTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class ConfirmPaymentRequest
        {
            public int? ConfirmedBy { get; set; }
        }
    }
}
