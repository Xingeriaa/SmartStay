using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace do_an_tot_nghiep.Controllers.Api
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardApiController : ControllerBase
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DashboardApiController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] string? monthYear)
        {
            if (string.IsNullOrEmpty(monthYear))
            {
                monthYear = DateTime.Now.ToString("MM/yyyy");
            }

            // Create separate scopes to execute queries in parallel against remote DB
            using var scopeRevenue = _scopeFactory.CreateScope();
            var ctxRevenue = scopeRevenue.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var taskRevenue = ctxRevenue.Invoices
                .Where(i => i.MonthYear == monthYear && i.Status == "Paid")
                .SumAsync(i => i.TotalAmount);

            using var scopeDebt = _scopeFactory.CreateScope();
            var ctxDebt = scopeDebt.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var taskDebtInvoices = ctxDebt.Invoices
                .Where(i => i.Status != "Paid" && i.Status != "Cancelled")
                .SumAsync(i => i.TotalAmount);
            var taskDebtLedger = ctxDebt.TenantBalances
                .Where(tb => tb.BalanceAmount < 0)
                .SumAsync(tb => -tb.BalanceAmount);

            using var scopeRooms = _scopeFactory.CreateScope();
            var ctxRooms = scopeRooms.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var taskTotalRooms = ctxRooms.PhongTros.CountAsync();
            var taskEmptyRooms = ctxRooms.PhongTros.CountAsync(p => p.Status == 1 || p.Status == 3);

            using var scopeMeter = _scopeFactory.CreateScope();
            var ctxMeter = scopeMeter.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var taskMeter = ctxMeter.MeterReadings
                .Where(m => m.MonthYear == monthYear)
                .ToListAsync();

            // Await all queries concurrently
            await Task.WhenAll(
                taskRevenue,
                taskDebtInvoices,
                taskDebtLedger,
                taskTotalRooms,
                taskEmptyRooms,
                taskMeter
            );

            decimal totalRevenue = taskRevenue.Result;
            decimal totalDebt = taskDebtInvoices.Result + taskDebtLedger.Result;
            
            int totalRooms = taskTotalRooms.Result;
            int emptyRooms = taskEmptyRooms.Result;
            decimal vacancyRate = totalRooms > 0 ? (decimal)emptyRooms / totalRooms * 100 : 0;

            var meterReadings = taskMeter.Result;
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
