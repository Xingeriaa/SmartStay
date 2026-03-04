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

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = 1,
                    Action = "CreateInvoice",
                    EntityName = "Invoices",
                    EntityId = invoice.Id.ToString(),
                    NewValue = $"Invoice {invoice.InvoiceCode} created manually",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                invoice.Details = details;
            }

            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        /// <summary>
        /// Tạo hóa đơn tự động từ Hợp đồng + Chỉ số điện nước + Cấu hình dịch vụ (Snapshot)
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateInvoiceRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var contract = await _context.HopDongs
                    .Include(c => c.HopDongKhachThues)
                    .FirstOrDefaultAsync(c => c.Id == request.ContractId);

                if (contract == null || contract.IsDeleted)
                    return BadRequest("Không tìm thấy hợp đồng hợp lệ.");

                bool hasInvoice = await _context.Invoices.AnyAsync(x => x.ContractId == contract.Id && x.MonthYear == request.MonthYear);
                if (hasInvoice) return BadRequest($"Hóa đơn kỳ {request.MonthYear} đã được tạo trước đó.");

                var rep = contract.HopDongKhachThues.FirstOrDefault(x => x.IsRepresentative);
                if (rep == null) return BadRequest("Hợp đồng thiếu người đại diện.");

                var invoice = new Invoice
                {
                    InvoiceCode = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{contract.Id}",
                    ContractId = contract.Id,
                    Period = "1 Tháng",
                    MonthYear = request.MonthYear,
                    DueDate = request.DueDate,
                    Status = "Unpaid",
                    CreatedAt = DateTime.UtcNow
                };

                var details = new List<InvoiceDetail>();

                // 1. Rent (Snapshot from Contract)
                details.Add(new InvoiceDetail
                {
                    Description = "Tiền phòng",
                    Quantity = 1,
                    UnitPriceSnapshot = contract.GiaThue,
                    Amount = contract.GiaThue
                });

                // 2. Services (Snapshot from ContractServices)
                var servicesQuery = await (from cs in _context.ContractServices
                                           join dv in _context.DichVu on cs.ServiceId equals dv.Id
                                           where cs.ContractId == contract.Id
                                           select new { cs, dv }).ToListAsync();

                var meterReading = await _context.MeterReadings
                    .FirstOrDefaultAsync(m => m.RoomId == contract.PhongTroId && m.MonthYear == request.MonthYear);

                foreach (var row in servicesQuery)
                {
                    decimal reqQty = row.cs.Quantity;

                    if (row.dv.LoaiDichVu == do_an_tot_nghiep.Models.Enums.ServiceChargeType.Metered)
                    {
                        if (row.dv.Ten.ToLower().Contains("điện"))
                        {
                            reqQty = meterReading?.ConsumptionElectricity ?? 0;
                        }
                        else if (row.dv.Ten.ToLower().Contains("nước"))
                        {
                            reqQty = meterReading?.ConsumptionWater ?? 0;
                        }
                    }
                    else if (row.dv.LoaiDichVu == do_an_tot_nghiep.Models.Enums.ServiceChargeType.FixedPerTenant)
                    {
                        // Calculate active tenants for the room
                        var tenantCount = await _context.HopDongKhachThues
                            .Where(h => h.HopDongId == contract.Id && h.KhachThue.IsDeleted == false)
                            .CountAsync();
                        reqQty = tenantCount;
                    }
                    else if (row.dv.LoaiDichVu == do_an_tot_nghiep.Models.Enums.ServiceChargeType.UsageBased)
                    {
                        var usageDateStart = DateTime.Parse(request.MonthYear);
                        var usage = await _context.MonthlyServiceUsages
                            .Where(u => u.ContractId == contract.Id && u.ServiceId == row.dv.Id && u.MonthYear.Year == usageDateStart.Year && u.MonthYear.Month == usageDateStart.Month)
                            .SumAsync(u => u.Quantity);
                        reqQty = usage;
                    }

                    if (reqQty > 0 || row.dv.LoaiDichVu == do_an_tot_nghiep.Models.Enums.ServiceChargeType.FixedPerRoom || row.dv.LoaiDichVu == do_an_tot_nghiep.Models.Enums.ServiceChargeType.Membership)
                    {
                        if (row.dv.LoaiDichVu == do_an_tot_nghiep.Models.Enums.ServiceChargeType.FixedPerRoom || row.dv.LoaiDichVu == do_an_tot_nghiep.Models.Enums.ServiceChargeType.Membership)
                        {
                            reqQty = row.cs.Quantity; // Reset to contract explicitly configured quantity
                        }

                        details.Add(new InvoiceDetail
                        {
                            Description = row.dv.Ten,
                            Quantity = reqQty,
                            UnitPriceSnapshot = row.cs.UnitPriceSnapshot,
                            Amount = reqQty * row.cs.UnitPriceSnapshot
                        });
                    }
                }

                decimal preTotal = details.Sum(x => x.Amount);

                // 3. Balance Ledger offset
                var balance = await _context.TenantBalances.FirstOrDefaultAsync(b => b.TenantId == rep.KhachThueId);
                decimal deltaBalance = 0;
                decimal offsetAmount = 0;

                if (balance != null && balance.BalanceAmount != 0)
                {
                    if (balance.BalanceAmount > 0)
                    {
                        // Credit. Use enough to pay the invoice, max = preTotal
                        if (balance.BalanceAmount >= preTotal)
                        {
                            offsetAmount = -preTotal;
                            deltaBalance = -preTotal;
                        }
                        else
                        {
                            offsetAmount = -balance.BalanceAmount;
                            deltaBalance = -balance.BalanceAmount;
                        }
                    }
                    else
                    {
                        // Debt. Add to invoice and zero the debt.
                        offsetAmount = Math.Abs(balance.BalanceAmount);
                        deltaBalance = Math.Abs(balance.BalanceAmount); // (Means balance -= (-)balance -> balance = 0)
                    }

                    if (offsetAmount != 0)
                    {
                        details.Add(new InvoiceDetail
                        {
                            Description = offsetAmount < 0 ? "Bù trừ bằng Số Dư Tín Dụng" : "Cộng dồn Nợ Cũ",
                            Quantity = 1,
                            UnitPriceSnapshot = offsetAmount,
                            Amount = offsetAmount
                        });
                    }
                }

                invoice.SubTotal = details.Sum(x => x.Amount);
                invoice.TotalAmount = invoice.SubTotal;

                if (invoice.TotalAmount <= 0)
                {
                    invoice.TotalAmount = 0;
                    invoice.Status = "Paid";
                    invoice.PaymentDate = DateTime.UtcNow;
                }

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(); // gets invoice.Id

                foreach (var d in details)
                {
                    d.InvoiceId = invoice.Id;
                }
                _context.InvoiceDetails.AddRange(details);

                if (deltaBalance != 0 && balance != null)
                {
                    decimal oldBalance = balance.BalanceAmount;
                    balance.BalanceAmount += deltaBalance; // Using deltaBalance. If offsetAmount=-preTotal => deltaBalance=-preTotal. Debt was -500 => deltaBalance=+500 => balance=0.
                    balance.LastUpdatedAt = DateTime.UtcNow;

                    _context.TenantBalanceTransactions.Add(new TenantBalanceTransaction
                    {
                        BalanceId = balance.Id,
                        TenantId = rep.KhachThueId,
                        Amount = deltaBalance,
                        TransactionType = "AutoOffset",
                        RelatedInvoiceId = invoice.Id,
                        BalanceBefore = oldBalance,
                        BalanceAfter = balance.BalanceAmount,
                        Reason = $"Cấn trừ hóa đơn {invoice.InvoiceCode}",
                        CreatedBy = 1,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // Track Generation AuditLog
                _context.AuditLogs.Add(new AuditLog
                {
                    EntityName = "Invoices",
                    EntityId = invoice.Id.ToString(),
                    Action = "Generate",
                    OldValue = "N/A",
                    NewValue = $"Generated total: {invoice.TotalAmount:N0} for {invoice.MonthYear}",
                    UserId = 1, // Assume created by System/Admin
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
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

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = 1,
                Action = "UpdateInvoiceStatus",
                EntityName = "Invoices",
                EntityId = invoice.Id.ToString(),
                NewValue = $"Status updated to {request.Status}",
                CreatedAt = DateTime.UtcNow
            });

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

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = 1,
                Action = "DeleteInvoice",
                EntityName = "Invoices",
                EntityId = invoice.Id.ToString(),
                OldValue = $"InvoiceCode: {invoice.InvoiceCode}",
                NewValue = "REMOVED (Hard Delete - System bypass)",
                CreatedAt = DateTime.UtcNow
            });

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

        public class GenerateInvoiceRequest
        {
            public int ContractId { get; set; }
            public string MonthYear { get; set; } = string.Empty;
            public DateTime DueDate { get; set; }
        }

        public class UpdateInvoiceStatusRequest
        {
            public string Status { get; set; } = "Unpaid";
            public DateTime? PaymentDate { get; set; }
        }
    }
}
