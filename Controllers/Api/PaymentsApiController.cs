using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace do_an_tot_nghiep.Controllers.Api
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PaymentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("manual-pay")]
        public async Task<IActionResult> ManualPay([FromBody] ManualPayRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = await _context.Invoices.FindAsync(request.InvoiceId);
                if (invoice == null) return NotFound("Hóa đơn không tồn tại.");

                if (invoice.Status == "Paid") return BadRequest("Hóa đơn đã được thanh toán.");

                var payTx = new PaymentTransaction
                {
                    InvoiceId = invoice.Id,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    BankName = request.BankName,
                    TransactionCode = string.IsNullOrWhiteSpace(request.TransactionCode) ? "N/A" : request.TransactionCode,
                    PaidAt = DateTime.UtcNow,
                    ConfirmedBy = 1 // Giả định ID user (Staff) đang thao tác
                };

                _context.PaymentTransactions.Add(payTx);

                // Update Invoice
                invoice.Status = "Paid";
                invoice.PaymentDate = DateTime.UtcNow;

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = 1,
                    Action = "Manual_Payment",
                    EntityName = "Invoices",
                    EntityId = invoice.Id.ToString(),
                    OldValue = "Unpaid",
                    NewValue = $"Paid {request.Amount:N0} via {request.PaymentMethod}",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Thanh toán thành công." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("sepay-webhook")]
        public async Task<IActionResult> SePayWebhook([FromBody] SePayWebhookPayload payload)
        {
            // Bảo mật: Nên check API Key ở Headers để xác thực (Tùy chọn)

            if (payload == null || string.IsNullOrWhiteSpace(payload.TransactionContent))
                return BadRequest("Invalid Payload");

            // Cách bóc tách InvoiceCode. Giả sử nội dung chuyển khoản là "Thanh toan HD INV-20231102143000-15"
            // Ta dùng Regex để tìm mã hóa đơn có cấu trúc "INV-[0-9]+-[0-9]+"
            var match = Regex.Match(payload.TransactionContent, @"INV-\d+-\d+");
            if (!match.Success)
            {
                return Ok(new { message = "Không tìm thấy mã Hóa đơn trong nội dung CK." });
            }

            string invoiceCode = match.Value;

            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceCode == invoiceCode);
            if (invoice == null)
                return Ok(new { message = $"Không tìm thấy Hóa đơn {invoiceCode} trong hệ thống." });

            if (invoice.Status == "Paid")
                return Ok(new { message = "Hóa đơn này đã được thanh toán rồi." });

            // Check số tiền
            if (payload.AmountIn < invoice.TotalAmount)
            {
                // Chưa thanh toán đủ, có thể xử lý chuyển nợ (TenantBalance) nhưng ở đây ví dụ đơn giản là chỉ MarkPaid nếu đủ
                return Ok(new { message = $"Thanh toán thiếu. Hóa đơn {invoice.TotalAmount}, CK {payload.AmountIn}" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var payTx = new PaymentTransaction
                {
                    InvoiceId = invoice.Id,
                    Amount = payload.AmountIn,
                    PaymentMethod = "BankTransfer_Webhook",
                    BankName = payload.Gateway,
                    TransactionCode = payload.ReferenceNumber ?? "AUTO_WEBHOOK",
                    PaidAt = DateTime.UtcNow
                };

                _context.PaymentTransactions.Add(payTx);

                invoice.Status = "Paid";
                invoice.PaymentDate = DateTime.UtcNow;

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = 0, // System
                    Action = "Auto_Webhook_Payment",
                    EntityName = "Invoices",
                    EntityId = invoice.Id.ToString(),
                    OldValue = "Unpaid",
                    NewValue = $"Auto Paid {payload.AmountIn:N0} via {payload.Gateway}",
                    CreatedAt = DateTime.UtcNow
                });

                // Nếu ck dư -> Bắn vào CreditBalance
                var diff = payload.AmountIn - invoice.TotalAmount;
                if (diff > 0)
                {
                    var hd_tenant = await _context.Set<HopDongKhachThue>().FirstOrDefaultAsync(k => k.HopDongId == invoice.ContractId);
                    int tenantId = hd_tenant?.KhachThueId ?? 0;
                    if (tenantId != 0)
                    {
                        var balance = await _context.TenantBalances.FirstOrDefaultAsync(b => b.TenantId == tenantId);
                        if (balance == null)
                        {
                            balance = new TenantBalance { TenantId = tenantId };
                            _context.TenantBalances.Add(balance);
                        }

                        decimal beforeAmount = balance.BalanceAmount;
                        balance.BalanceAmount += diff;
                        balance.LastUpdatedAt = DateTime.UtcNow;

                        _context.TenantBalanceTransactions.Add(new TenantBalanceTransaction
                        {
                            BalanceId = balance.Id,
                            TenantId = tenantId,
                            Amount = diff,
                            TransactionType = "Overpayment",
                            RelatedInvoiceId = invoice.Id,
                            RelatedPaymentId = payTx.Id,
                            BalanceBefore = beforeAmount,
                            BalanceAfter = balance.BalanceAmount,
                            Reason = $"Tiền thừa từ Webhook chuyển khoản cho {invoice.InvoiceCode}",
                            CreatedBy = 0,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { success = true, message = $"Hóa đơn {invoiceCode} đã tự động đổi trạng thái." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class ManualPayRequest
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? BankName { get; set; }
        public string? TransactionCode { get; set; }
    }

    public class SePayWebhookPayload
    {
        public string Gateway { get; set; } = string.Empty;
        public decimal AmountIn { get; set; }
        public decimal AmountOut { get; set; }
        public string TransactionContent { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
    }
}
