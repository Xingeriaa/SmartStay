using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Tác vụ hệ thống theo thời gian (lập hóa đơn định kỳ, cảnh báo hợp đồng).
    /// </summary>
    [ApiController]
    [Route("api/system-tasks")]
    public class SystemTasksApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SystemTasksApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sinh hóa đơn nháp theo tháng cho các hợp đồng đang hiệu lực.
        /// monthYear dạng YYYY-MM, dueDate dạng YYYY-MM-DD.
        /// </summary>
        [HttpPost("generate-invoices")]
        public async Task<ActionResult<GenerateInvoicesResult>> GenerateInvoices([FromQuery] string monthYear, [FromQuery] DateTime dueDate)
        {
            if (string.IsNullOrWhiteSpace(monthYear))
            {
                return BadRequest("monthYear là bắt buộc (YYYY-MM).");
            }

            var contracts = await _context.HopDongs
                .Where(x => x.TrangThai == TrangThaiHopDong.DangHieuLuc)
                .ToListAsync();

            var created = 0;
            foreach (var contract in contracts)
            {
                var exists = await _context.Invoices
                    .AnyAsync(x => x.ContractId == contract.Id && x.MonthYear == monthYear);
                if (exists)
                {
                    continue;
                }

                var invoice = new Invoice
                {
                    InvoiceCode = $"INV-{monthYear}-{contract.Id}",
                    ContractId = contract.Id,
                    Period = monthYear,
                    MonthYear = monthYear,
                    SubTotal = contract.GiaThue,
                    TaxAmount = 0,
                    TotalAmount = contract.GiaThue,
                    DueDate = dueDate,
                    Status = "Unpaid",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                var detail = new InvoiceDetail
                {
                    InvoiceId = invoice.Id,
                    Description = "Tiền thuê phòng",
                    Quantity = 1,
                    UnitPriceSnapshot = contract.GiaThue,
                    Amount = contract.GiaThue
                };

                _context.InvoiceDetails.Add(detail);
                await _context.SaveChangesAsync();

                created++;
            }

            return new GenerateInvoicesResult
            {
                MonthYear = monthYear,
                CreatedInvoices = created,
                TotalContracts = contracts.Count
            };
        }

        /// <summary>
        /// Danh sách hợp đồng sắp hết hạn trong N ngày.
        /// </summary>
        [HttpGet("contracts-expiring")]
        public async Task<ActionResult<List<HopDong>>> GetContractsExpiring([FromQuery] int withinDays = 30)
        {
            var today = DateTime.Today;
            var limit = today.AddDays(withinDays);

            var contracts = await _context.HopDongs
                .Where(x => x.NgayKetThuc.HasValue && x.NgayKetThuc.Value <= limit && x.NgayKetThuc.Value >= today)
                .ToListAsync();

            return contracts;
        }

        /// <summary>
        /// Gửi thông báo nhắc tiền cho hóa đơn chưa thanh toán (có thể lọc theo monthYear).
        /// </summary>
        [HttpPost("send-payment-reminders")]
        public async Task<ActionResult<SendRemindersResult>> SendPaymentReminders([FromQuery] string? monthYear)
        {
            var invoicesQuery = _context.Invoices
                .Where(x => x.Status == "Unpaid" || x.Status == "Overdue");

            if (!string.IsNullOrWhiteSpace(monthYear))
            {
                invoicesQuery = invoicesQuery.Where(x => x.MonthYear == monthYear);
            }

            var invoices = await invoicesQuery.ToListAsync();
            var created = 0;

            foreach (var invoice in invoices)
            {
                var representative = await _context.HopDongKhachThues
                    .Where(x => x.HopDongId == invoice.ContractId && x.IsRepresentative)
                    .Select(x => x.KhachThue)
                    .FirstOrDefaultAsync();

                if (representative?.UserId == null)
                {
                    continue;
                }

                var notification = new Notification
                {
                    UserId = representative.UserId.Value,
                    Title = "Nhắc thanh toán hóa đơn",
                    Content = $"Hóa đơn {invoice.InvoiceCode} kỳ {invoice.MonthYear} chưa được thanh toán.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                created++;
            }

            await _context.SaveChangesAsync();

            return new SendRemindersResult
            {
                MonthYear = monthYear,
                NotificationsCreated = created
            };
        }

        public class GenerateInvoicesResult
        {
            public string MonthYear { get; set; } = string.Empty;
            public int CreatedInvoices { get; set; }
            public int TotalContracts { get; set; }
        }

        public class SendRemindersResult
        {
            public string? MonthYear { get; set; }
            public int NotificationsCreated { get; set; }
        }
    }
}
