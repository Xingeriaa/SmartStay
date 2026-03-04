using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using do_an_tot_nghiep.Filters; // Thêm namespace filter

namespace do_an_tot_nghiep.Controllers.Api
{
    [ApiController]
    [Route("api/dashboard")]
    [RequireRole(Roles.AdminOrStaff)] // Chuyển từ [Authorize] sang [RequireRole] để ăn Session cookies
    public class DashboardApiController(ApplicationDbContext ctx, ILogger<DashboardApiController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _ctx = ctx;
        private readonly ILogger<DashboardApiController> _logger = logger;

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] string? monthYear)
        {
            try
            {
                monthYear = NormalizeMonthYear(monthYear);

                // 1. Lọc thống kê theo IsDeleted = false cho phòng, dùng AsNoTracking để tối ưu (Database Optimizer)
                int totalRooms = await _ctx.PhongTros
                    .AsNoTracking()
                    .Where(p => p.IsDeleted == false)
                    .CountAsync();

                int emptyRooms = await _ctx.PhongTros
                    .AsNoTracking()
                    .Where(p => p.IsDeleted == false && (p.Status == 1 || p.Status == 3))
                    .CountAsync();

                // 2. Doanh thu & Công nợ (Revenue & Debt)
                // Theo Financial Rules của BMS: Không cộng đè mà phải lấy từ Transaction/Invoice đã chốt
                decimal totalRevenue = await _ctx.Invoices
                    .AsNoTracking()
                    .Where(i => i.MonthYear == monthYear && i.Status == "Paid")
                    .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;

                decimal debtInvoices = await _ctx.Invoices
                    .AsNoTracking()
                    .Where(i => i.Status != "Paid" && i.Status != "Cancelled")
                    .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;

                // Nợ lũy kế từ ví Tenant (Số âm là đang nợ hệ thống)
                decimal debtLedger = await _ctx.TenantBalances
                    .AsNoTracking()
                    .Where(tb => tb.BalanceAmount < 0)
                    .SumAsync(tb => (decimal?)-tb.BalanceAmount) ?? 0;

                // 3. Chỉ số điện nước (Meter Reading Safety)
                var meterReadings = await _ctx.MeterReadings
                    .AsNoTracking()
                    .Where(m => m.MonthYear == monthYear)
                    .ToListAsync();

                long totalElectricity = meterReadings.Sum(m => Math.Max(0, m.NewElectricityIndex - m.OldElectricityIndex));
                long totalWater = meterReadings.Sum(m => Math.Max(0, m.NewWaterIndex - m.OldWaterIndex));

                decimal totalDebt = debtInvoices + debtLedger;
                decimal vacancyRate = totalRooms > 0 ? (decimal)emptyRooms / totalRooms * 100 : 0;

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu Summary cho Dashboard: {MonthYear}", monthYear);
                return StatusCode(500, new { message = "Lỗi internal server khi load dữ liệu tổng quan." });
            }
        }

        [HttpGet("chart")]
        public async Task<IActionResult> GetChartData([FromQuery] string? monthYear)
        {
            try
            {
                monthYear = NormalizeMonthYear(monthYear);

                if (!DateTime.TryParse($"{monthYear}-01", out DateTime currentMonth))
                {
                    currentMonth = DateTime.Now;
                }

                var targetMonths = new List<string>();
                for (int i = 5; i >= 0; i--)
                {
                    targetMonths.Add(currentMonth.AddMonths(-i).ToString("yyyy-MM"));
                }

                // Tối ưu hoá vòng lặp N+1 (Database Optimizer Skill)
                // Lấy CÙNG LÚC data của 6 tháng thay vì await từng tháng rẽ nhánh, dùng AsNoTracking
                var invoicesData = await _ctx.Invoices
                    .AsNoTracking()
                    .Where(x => x.MonthYear != null && targetMonths.Contains(x.MonthYear))
                    .GroupBy(x => x.MonthYear)
                    .Select(g => new
                    {
                        Month = g.Key!,
                        Revenue = g.Where(x => x.Status == "Paid").Sum(x => (decimal?)x.TotalAmount) ?? 0,
                        Debt = g.Where(x => x.Status != "Paid" && x.Status != "Cancelled").Sum(x => (decimal?)x.TotalAmount) ?? 0
                    })
                    .ToDictionaryAsync(x => x.Month, x => x);

                var labels = new List<string>();
                var revenues = new List<decimal>();
                var debts = new List<decimal>();

                foreach (var month in targetMonths)
                {
                    labels.Add(month);
                    if (invoicesData.TryGetValue(month, out var data))
                    {
                        revenues.Add(data.Revenue);
                        debts.Add(data.Debt);
                    }
                    else
                    {
                        revenues.Add(0);
                        debts.Add(0);
                    }
                }

                return Ok(new
                {
                    labels,
                    revenues,
                    debts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi sinh biểu đồ Dashboard 6 tháng. MonthYear khởi điểm: {MonthYear}", monthYear);
                return StatusCode(500, new { message = "Lỗi khi vẽ biểu đồ doanh thu." });
            }
        }

        private static string NormalizeMonthYear(string? monthYear)
        {
            if (string.IsNullOrEmpty(monthYear))
            {
                return DateTime.Now.ToString("yyyy-MM");
            }

            if (monthYear.Contains('/'))
            {
                var parts = monthYear.Split('/');
                if (parts.Length == 2 && parts[0].Length == 2 && parts[1].Length == 4)
                {
                    return $"{parts[1]}-{parts[0]}";
                }
            }

            return monthYear;
        }
    }
}
