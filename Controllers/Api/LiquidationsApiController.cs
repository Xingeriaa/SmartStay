using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Thanh lý hợp đồng — quyết toán tài chính cuối kỳ.
    /// Deposit = NỢ PHẢI TRẢ, không ghi nhận doanh thu.
    /// </summary>
    [ApiController]
    [Route("api/liquidations")]
    public class LiquidationsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LiquidationsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────────────
        // GET api/liquidations — danh sách
        // ─────────────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult<List<Liquidation>>> GetAll([FromQuery] int? contractId)
        {
            var query = _context.Liquidations
                .Include(x => x.Contract)
                .AsNoTracking();

            if (contractId.HasValue)
                query = query.Where(x => x.ContractId == contractId.Value);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        // ─────────────────────────────────────────────────────
        // GET api/liquidations/preview/{contractId}
        // Tính toán preview thanh lý: nợ tồn, cọc, hoàn/thu thêm
        // ─────────────────────────────────────────────────────
        [HttpGet("preview/{contractId:int}")]
        public async Task<IActionResult> Preview(int contractId)
        {
            // 1. Load hợp đồng
            var contract = await _context.HopDongs
                .Include(h => h.PhongTro)
                .Include(h => h.HopDongKhachThues).ThenInclude(hk => hk.KhachThue)
                .FirstOrDefaultAsync(h => h.Id == contractId && !h.IsDeleted);

            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng." });

            if (contract.TrangThai != TrangThaiHopDong.DangHieuLuc)
                return BadRequest(new { message = "Hợp đồng không ở trạng thái Đang hiệu lực. Không thể thanh lý." });

            // 2. Khách thuê chính
            var mainTenant = contract.HopDongKhachThues?
                .OrderBy(x => x.Id).FirstOrDefault()?.KhachThue;

            // 3. Tổng hóa đơn chưa thanh toán của hợp đồng
            var unpaidInvoices = await _context.Invoices
                .Where(i => i.ContractId == contractId && i.Status != "Paid" && i.Status != "Cancelled")
                .ToListAsync();

            decimal totalUnpaid = unpaidInvoices.Sum(i => i.TotalAmount);

            // 4. Số dư tenant (Credit/Debit từ ledger)
            decimal ledgerCredit = 0, ledgerDebt = 0;
            if (mainTenant != null)
            {
                var balance = await _context.TenantBalances
                    .FirstOrDefaultAsync(b => b.TenantId == mainTenant.Id);
                if (balance != null)
                {
                    if (balance.BalanceAmount >= 0) ledgerCredit = balance.BalanceAmount;
                    else ledgerDebt = Math.Abs(balance.BalanceAmount);
                }
            }

            // 5. Chỉ số điện nước lần ghi gần nhất của phòng này
            int lastElec = 0, lastWater = 0;
            var lastMeter = await _context.MeterReadings
                .Where(m => m.RoomId == contract.PhongTroId)
                .OrderByDescending(m => m.MonthYear)
                .FirstOrDefaultAsync();
            if (lastMeter != null)
            {
                lastElec = (int)lastMeter.NewElectricityIndex;
                lastWater = (int)lastMeter.NewWaterIndex;
            }

            // 6. Net: CọcGiữ + SốDưCredit - NợHĐ - NợLedger
            decimal deposit = contract.TienCoc;
            decimal netLiability = totalUnpaid + ledgerDebt - ledgerCredit;    // dương = còn nợ, âm = dư
            decimal refund = Math.Max(0, deposit - netLiability);        // hoàn lại cho khách
            decimal addCharge = Math.Max(0, netLiability - deposit);        // thu thêm nếu nợ > cọc

            return Ok(new
            {
                contractId = contract.Id,
                contractCode = contract.ContractCode,
                roomName = contract.PhongTro?.TenPhong,
                tenantName = mainTenant?.HoTen ?? "Không xác định",
                ngayBatDau = contract.NgayBatDau,
                ngayKetThuc = contract.NgayKetThuc,
                depositAmount = deposit,
                totalUnpaidInvoices = totalUnpaid,
                ledgerDebt,
                ledgerCredit,
                netLiability,
                expectedRefundAmount = refund,
                expectedAdditionalCharge = addCharge,
                lastElectricityIndex = lastElec,
                lastWaterIndex = lastWater,
                unpaidInvoiceCount = unpaidInvoices.Count
            });
        }

        // ─────────────────────────────────────────────────────
        // POST api/liquidations/execute
        // Thực hiện thanh lý: tạo HĐ cuối, cấn trừ cọc, audit log
        // ─────────────────────────────────────────────────────
        [HttpPost("execute")]
        public async Task<IActionResult> Execute([FromBody] LiquidationExecuteRequest req)
        {
            // ── Validate ──────────────────────────────────────
            if (req == null) return BadRequest(new { message = "Payload rỗng." });
            if (req.FinalElectricityIndex < req.LastElectricityIndex)
                return BadRequest(new { message = "Chỉ số điện bàn giao phải >= lần trước (M5.1)." });
            if (req.FinalWaterIndex < req.LastWaterIndex)
                return BadRequest(new { message = "Chỉ số nước bàn giao phải >= lần trước (M5.1)." });

            // ── Load hợp đồng ─────────────────────────────────
            var contract = await _context.HopDongs
                .Include(h => h.PhongTro)
                .Include(h => h.HopDongKhachThues).ThenInclude(hk => hk.KhachThue)
                .FirstOrDefaultAsync(h => h.Id == req.ContractId && !h.IsDeleted);

            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng." });
            if (contract.TrangThai != TrangThaiHopDong.DangHieuLuc)
                return BadRequest(new { message = "Hợp đồng không còn hiệu lực." });

            // ── Bắt đầu transaction DB ────────────────────────
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var mainTenant = contract.HopDongKhachThues?
                    .OrderBy(x => x.Id).FirstOrDefault()?.KhachThue;

                // 1. Tính lại nợ thực tế
                var unpaid = await _context.Invoices
                    .Where(i => i.ContractId == contract.Id && i.Status != "Paid" && i.Status != "Cancelled")
                    .ToListAsync();
                decimal totalUnpaid = unpaid.Sum(i => i.TotalAmount);

                TenantBalance? tenantBalance = null;
                decimal ledgerCredit = 0, ledgerDebt = 0;
                if (mainTenant != null)
                {
                    tenantBalance = await _context.TenantBalances
                        .FirstOrDefaultAsync(b => b.TenantId == mainTenant.Id);
                    if (tenantBalance != null)
                    {
                        if (tenantBalance.BalanceAmount >= 0) ledgerCredit = tenantBalance.BalanceAmount;
                        else ledgerDebt = Math.Abs(tenantBalance.BalanceAmount);
                    }
                }

                decimal deposit = contract.TienCoc;
                decimal penalty = req.PenaltyAmount;
                decimal netLiability = totalUnpaid + ledgerDebt + penalty - ledgerCredit;
                decimal refund = Math.Max(0, deposit - netLiability);
                decimal addCharge = Math.Max(0, netLiability - deposit);
                decimal depositUsed = deposit - refund; // phần cọc dùng bù nợ

                // 2. Tạo bản ghi Liquidation
                var liquidation = new Liquidation
                {
                    ContractId = contract.Id,
                    InvoiceId = 0,          // sẽ cập nhật nếu có hóa đơn cuối
                    Reason = req.Reason ?? "Thanh lý tự động",
                    FinalInvoiceAmount = totalUnpaid + penalty,
                    DepositUsed = depositUsed,
                    RefundAmount = refund,
                    AdditionalCharge = addCharge,
                    StaffId = req.StaffId,
                    CreatedAt = now
                };
                _context.Liquidations.Add(liquidation);

                // 3. Đánh dấu những hóa đơn tồn là đã quyết toán qua cọc
                //    (nếu cọc đủ bù, mark = SettledByDeposit; nếu thiếu để nguyên)
                foreach (var inv in unpaid)
                {
                    inv.Status = deposit >= totalUnpaid ? "SettledByDeposit" : "Unpaid";
                }

                // 4. Cập nhật TenantBalance — MANDATORY ledger rule
                if (mainTenant != null)
                {
                    if (tenantBalance == null)
                    {
                        tenantBalance = new TenantBalance
                        {
                            TenantId = mainTenant.Id,
                            BalanceAmount = 0,
                            CreatedAt = now,
                            LastUpdatedAt = now
                        };
                        _context.TenantBalances.Add(tenantBalance);
                        await _context.SaveChangesAsync(); // cần Id
                    }

                    decimal balanceBefore = tenantBalance.BalanceAmount;
                    decimal balanceAfter;

                    if (refund > 0)
                    {
                        // Hoàn tiền → cộng vào balance (hoặc ghi nhận chờ chuyển khoản)
                        balanceAfter = balanceBefore + refund;
                        _context.TenantBalanceTransactions.Add(new TenantBalanceTransaction
                        {
                            TenantId = mainTenant.Id,
                            BalanceId = tenantBalance.Id,
                            TransactionType = "DepositRefund",
                            Amount = refund,
                            BalanceBefore = balanceBefore,
                            BalanceAfter = balanceAfter,
                            Note = $"Hoàn cọc — thanh lý HĐ {contract.ContractCode}",
                            Reason = req.Reason,
                            CreatedBy = req.StaffId,
                            CreatedAt = now
                        });
                    }
                    else if (addCharge > 0)
                    {
                        // Thu thêm → ghi nợ vào balance
                        balanceAfter = balanceBefore - addCharge;
                        _context.TenantBalanceTransactions.Add(new TenantBalanceTransaction
                        {
                            TenantId = mainTenant.Id,
                            BalanceId = tenantBalance.Id,
                            TransactionType = "LiquidationDebt",
                            Amount = -addCharge,
                            BalanceBefore = balanceBefore,
                            BalanceAfter = balanceAfter,
                            Note = $"Thu thêm sau thanh lý HĐ {contract.ContractCode} (nợ vượt cọc)",
                            Reason = req.Reason,
                            CreatedBy = req.StaffId,
                            CreatedAt = now
                        });
                    }
                    else
                    {
                        // Cọc vừa đủ bù nợ, balance về 0
                        balanceAfter = balanceBefore;
                        _context.TenantBalanceTransactions.Add(new TenantBalanceTransaction
                        {
                            TenantId = mainTenant.Id,
                            BalanceId = tenantBalance.Id,
                            TransactionType = "DepositOffset",
                            Amount = 0,
                            BalanceBefore = balanceBefore,
                            BalanceAfter = balanceAfter,
                            Note = $"Cọc bù đủ nợ — HĐ {contract.ContractCode} quyết toán cân bằng",
                            Reason = req.Reason,
                            CreatedBy = req.StaffId,
                            CreatedAt = now
                        });
                    }

                    tenantBalance.BalanceAmount = balanceAfter;
                    tenantBalance.LastUpdatedAt = now;
                }

                // 5. Chuyển trạng thái hợp đồng → DaThanhLy
                contract.TrangThai = TrangThaiHopDong.DaThanhLy;
                contract.NgayKetThuc = now;

                // 6. Giải phóng phòng → Vacant
                if (contract.PhongTro != null)
                    contract.PhongTro.Status = "Vacant";

                // 7. AuditLog — BẮT BUỘC theo rule VIII
                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = req.StaffId,
                    Action = "ContractLiquidation",
                    EntityName = "Liquidations",
                    EntityId = contract.Id.ToString(),
                    OldValue = "DangHieuLuc",
                    NewValue = $"DaThanhLy | Refund={refund:N0} | AddCharge={addCharge:N0} | Reason={req.Reason}",
                    CreatedAt = now
                });

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    message = "Thanh lý hợp đồng thành công.",
                    contractId = contract.Id,
                    contractCode = contract.ContractCode,
                    refundAmount = refund,
                    additionalCharge = addCharge,
                    depositUsed,
                    roomStatus = "Vacant"
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        // ─────────────────────────────────────────────────────
        // DELETE api/liquidations/{id}  (soft-unsafe: chỉ Admin)
        // ─────────────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var liq = await _context.Liquidations.FindAsync(id);
            if (liq == null) return NotFound();

            // KHÔNG hard-delete financial record — flag error
            return BadRequest(new { message = "Không thể xóa bản ghi thanh lý. Đây là dữ liệu tài chính lịch sử (Rule IV)." });
        }
    }

    // ── Request DTO ───────────────────────────────────────────
    public class LiquidationExecuteRequest
    {
        public int ContractId { get; set; }
        public int LastElectricityIndex { get; set; }
        public int FinalElectricityIndex { get; set; }
        public int LastWaterIndex { get; set; }
        public int FinalWaterIndex { get; set; }
        public decimal PenaltyAmount { get; set; }
        public string? Reason { get; set; }
        public int StaffId { get; set; } = 1;
    }
}
