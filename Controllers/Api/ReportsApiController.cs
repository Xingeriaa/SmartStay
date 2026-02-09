using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Báo cáo &amp; thống kê tổng quan hệ thống.
    /// </summary>
    [ApiController]
    [Route("api/reports")]
    public class ReportsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Dashboard tổng quan: số tòa nhà, phòng, cư dân, hợp đồng, hóa đơn chưa thanh toán.
        /// </summary>
        [HttpGet("overview")]
        public async Task<ActionResult<OverviewReport>> GetOverview()
        {
            var result = new OverviewReport
            {
                Buildings = await _context.NhaTros.CountAsync(),
                Rooms = await _context.PhongTros.CountAsync(),
                Tenants = await _context.KhachThues.CountAsync(),
                Contracts = await _context.HopDongs.CountAsync(),
                UnpaidInvoices = await _context.Invoices.CountAsync(x => x.Status == "Unpaid" || x.Status == "Overdue")
            };

            return result;
        }

        /// <summary>
        /// Thống kê doanh thu theo tháng (MonthYear dạng YYYY-MM).
        /// </summary>
        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueReport>> GetRevenue([FromQuery] string monthYear)
        {
            var total = await _context.Invoices
                .Where(x => x.MonthYear == monthYear && x.Status == "Paid")
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0m;

            return new RevenueReport { MonthYear = monthYear, TotalRevenue = total };
        }

        /// <summary>
        /// Thống kê công nợ (tổng tiền hóa đơn chưa thanh toán).
        /// </summary>
        [HttpGet("arrears")]
        public async Task<ActionResult<ArrearsReport>> GetArrears()
        {
            var total = await _context.Invoices
                .Where(x => x.Status == "Unpaid" || x.Status == "Overdue")
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0m;

            return new ArrearsReport { TotalArrears = total };
        }

        /// <summary>
        /// Thống kê tỷ lệ phòng trống.
        /// </summary>
        [HttpGet("vacancy")]
        public async Task<ActionResult<VacancyReport>> GetVacancy()
        {
            var totalRooms = await _context.PhongTros.CountAsync();
            var vacantRooms = await _context.PhongTros.CountAsync(x => x.Status == "Vacant");

            return new VacancyReport
            {
                TotalRooms = totalRooms,
                VacantRooms = vacantRooms
            };
        }

        public class OverviewReport
        {
            public int Buildings { get; set; }
            public int Rooms { get; set; }
            public int Tenants { get; set; }
            public int Contracts { get; set; }
            public int UnpaidInvoices { get; set; }
        }

        public class RevenueReport
        {
            public string MonthYear { get; set; } = string.Empty;
            public decimal TotalRevenue { get; set; }
        }

        public class ArrearsReport
        {
            public decimal TotalArrears { get; set; }
        }

        public class VacancyReport
        {
            public int TotalRooms { get; set; }
            public int VacantRooms { get; set; }
        }
    }
}
