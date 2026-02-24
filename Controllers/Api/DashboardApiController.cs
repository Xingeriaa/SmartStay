using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace do_an_tot_nghiep.Controllers.Api
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] string? monthYear)
        {
            if (string.IsNullOrEmpty(monthYear))
            {
                monthYear = DateTime.Now.ToString("MM/yyyy");
            }

            // 1. Doanh thu theo tháng (Invoices đã Payment trong kì)
            var paidInvoices = await _context.Invoices
                .Where(i => i.MonthYear == monthYear && i.Status == "Paid")
                .ToListAsync();

            decimal totalRevenue = paidInvoices.Sum(i => i.TotalAmount);

            // 2. Công nợ (Tất cả Invoices Unpaid/Overdue + TotalDebt Sổ Cái)
            var unpaidInvoicesSum = await _context.Invoices
                .Where(i => i.Status != "Paid" && i.Status != "Cancelled")
                .SumAsync(i => i.TotalAmount);
            var ledgerDebtSum = await _context.TenantBalances.Where(tb => tb.BalanceAmount < 0).SumAsync(tb => -tb.BalanceAmount);

            decimal totalDebt = unpaidInvoicesSum + ledgerDebtSum;

            // 3. Tỷ lệ phòng trống
            var totalRooms = await _context.PhongTros.CountAsync();
            var emptyRooms = await _context.PhongTros.CountAsync(p => p.Status == "Empty" || p.Status == "Vacant" || p.Status == "Maintenance");

            decimal vacancyRate = totalRooms > 0 ? (decimal)emptyRooms / totalRooms * 100 : 0;

            // 4. Tiêu thụ điện nước tháng hiện tại
            var meterReadings = await _context.MeterReadings
                .Where(m => m.MonthYear == monthYear)
                .ToListAsync();

            long totalElectricity = meterReadings.Sum(m => m.NewElectricityIndex - m.OldElectricityIndex);
            long totalWater = meterReadings.Sum(m => m.NewWaterIndex - m.OldWaterIndex);

            return Ok(new
            {
                Revenue = totalRevenue,
                Debt = totalDebt,
                TotalRooms = totalRooms,
                EmptyRooms = emptyRooms,
                VacancyRate = Math.Round(vacancyRate, 2),
                ElectricityConsumption = totalElectricity,
                WaterConsumption = totalWater,
                MonthYear = monthYear
            });
        }
    }
}
