namespace do_an_tot_nghiep.ViewModels
{
    /// <summary>
    /// ViewModel hiển thị preview quyết toán thanh lý.
    /// Map với response từ GET api/liquidations/preview/{contractId}.
    /// </summary>
    public class LiquidationPreviewViewModel
    {
        public int ContractId { get; set; }
        public string? ContractCode { get; set; }
        public string? RoomName { get; set; }
        public string? TenantName { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        // ── Tài chính ──────────────────────────────────────
        public decimal DepositAmount { get; set; }          // Cọc đang giữ (Liability)
        public decimal TotalUnpaidInvoices { get; set; }    // Tổng HĐ chưa trả
        public decimal LedgerDebt { get; set; }             // Nợ sổ cái (balance < 0)
        public decimal LedgerCredit { get; set; }           // Dư sổ cái (balance > 0)
        public decimal NetLiability { get; set; }           // Nợ ròng = Unpaid + Debt - Credit
        public decimal ExpectedRefundAmount { get; set; }   // Cọc hoàn lại
        public decimal ExpectedAdditionalCharge { get; set; } // Thu thêm nếu nợ > cọc

        // ── Điện nước ──────────────────────────────────────
        public int LastElectricityIndex { get; set; }
        public int LastWaterIndex { get; set; }
        public int UnpaidInvoiceCount { get; set; }
    }

    /// <summary>
    /// ViewModel nhận form người dùng submit thực hiện thanh lý.
    /// </summary>
    public class LiquidationConfirmViewModel
    {
        public int ContractId { get; set; }

        // Meter readings
        public int LastElectricityIndex { get; set; }
        public int FinalElectricityIndex { get; set; }
        public int LastWaterIndex { get; set; }
        public int FinalWaterIndex { get; set; }

        // Penalty & reason
        public decimal PenaltyAmount { get; set; }
        public string? Reason { get; set; }
    }
}
