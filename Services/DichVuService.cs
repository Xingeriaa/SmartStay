using do_an_tot_nghiep.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Services
{
    public interface IDichVuService
    {
        Task<List<DichVu>> GetServicesAsync(string? searchString);
        List<string> GetLoaiDichVuList();
        Task CreateAsync(DichVu model);
        Task<DichVu?> GetForEditAsync(int id);
        Task<bool> UpdateAsync(DichVu model);
        Task DeleteAsync(int id);
        Task<DeleteSelectedResult> DeleteSelectedAsync(List<int> ids);
    }

    public sealed record DeleteSelectedResult(bool Success, string? Message);

    public class DichVuService : IDichVuService
    {
        private readonly ApplicationDbContext _context;

        public DichVuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DichVu>> GetServicesAsync(string? searchString)
        {
            var services = await _context.DichVu
                .AsNoTracking()
                .ToListAsync();

            var serviceIds = services.Select(s => s.Id).ToList();

            var priceHistory = await _context.ServicePriceHistory
                .Where(p => serviceIds.Contains(p.ServiceId) && p.IsActive)
                .ToListAsync();

            var latestPrices = priceHistory
                .GroupBy(p => p.ServiceId)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var service in services)
            {
                if (latestPrices.TryGetValue(service.Id, out var latest))
                {
                    service.DonGia = latest.UnitPrice;
                }
                else
                {
                    service.DonGia = 0;
                }
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                services = services.Where(s => s.Ten.Contains(searchString)).ToList();
            }

            return services;
        }

        public List<string> GetLoaiDichVuList()
        {
            return new List<string> { "Tiền điện", "Tiền nước", "Tiền rác", "Wifi", "Gửi xe" };
        }

        public async Task CreateAsync(DichVu model)
        {
            _context.DichVu.Add(model);
            await _context.SaveChangesAsync();

            if (model.DonGia > 0)
            {
                var price = new ServicePriceHistory
                {
                    ServiceId = model.Id,
                    UnitPrice = model.DonGia,
                    EffectiveFrom = DateTime.Today,
                    IsActive = true
                };
                _context.ServicePriceHistory.Add(price);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DichVu?> GetForEditAsync(int id)
        {
            var dichVu = await _context.DichVu.FindAsync(id);
            if (dichVu == null) return null;

            var latestPrice = await _context.ServicePriceHistory
                .Where(p => p.ServiceId == dichVu.Id && p.IsActive)
                .FirstOrDefaultAsync();
            dichVu.DonGia = latestPrice?.UnitPrice ?? 0;

            return dichVu;
        }

        public async Task<bool> UpdateAsync(DichVu model)
        {
            var entity = await _context.DichVu.FindAsync(model.Id);
            if (entity == null) return false;

            entity.Ten = model.Ten;
            entity.LoaiDichVu = model.LoaiDichVu;
            entity.IsActive = model.IsActive;
            entity.IsDeleted = model.IsDeleted;

            var currentPrice = await _context.ServicePriceHistory
                .Where(p => p.ServiceId == model.Id && p.IsActive)
                .FirstOrDefaultAsync();

            if (currentPrice == null || currentPrice.UnitPrice != model.DonGia)
            {
                var newEffectiveDate = DateTime.Today;

                if (currentPrice != null)
                {
                    currentPrice.EffectiveTo = newEffectiveDate.AddDays(-1);
                    currentPrice.IsActive = false;
                }

                var newPrice = new ServicePriceHistory
                {
                    ServiceId = model.Id,
                    UnitPrice = model.DonGia,
                    EffectiveFrom = newEffectiveDate,
                    IsActive = true
                };
                _context.ServicePriceHistory.Add(newPrice);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.DichVu.FindAsync(id);
            if (item != null)
            {
                var prices = _context.ServicePriceHistory.Where(p => p.ServiceId == id);
                _context.ServicePriceHistory.RemoveRange(prices);
                _context.DichVu.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DeleteSelectedResult> DeleteSelectedAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new DeleteSelectedResult(false, "Không có mục nào được chọn.");
            }

            var itemsToDelete = await _context.DichVu.Where(x => ids.Contains(x.Id)).ToListAsync();
            if (itemsToDelete.Any())
            {
                var priceToDelete = _context.ServicePriceHistory.Where(p => ids.Contains(p.ServiceId));
                _context.ServicePriceHistory.RemoveRange(priceToDelete);
                _context.DichVu.RemoveRange(itemsToDelete);
                await _context.SaveChangesAsync();
            }

            return new DeleteSelectedResult(true, null);
        }
    }
}
