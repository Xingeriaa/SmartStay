using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Services
{
    public interface IKhachThueService
    {
        Task<List<KhachThue>> GetAllAsync();
        Task<KhachThue?> GetByIdAsync(int id);
        Task CreateAsync(KhachThue model, IFormFile? hinhAnh);
        Task<bool> UpdateAsync(int id, KhachThue model, IFormFile? hinhAnh);
        Task DeleteAsync(int id);
    }

    public class KhachThueService : IKhachThueService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public KhachThueService(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public Task<List<KhachThue>> GetAllAsync()
        {
            return _context.KhachThues.ToListAsync();
        }

        public Task<KhachThue?> GetByIdAsync(int id)
        {
            return _context.KhachThues.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(KhachThue model, IFormFile? hinhAnh)
        {
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(hinhAnh.FileName);
                string path = Path.Combine(wwwRootPath, "images", "khachthue");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(fileStream);
                }

                model.HinhAnh = fileName;
            }

            _context.KhachThues.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, KhachThue model, IFormFile? hinhAnh)
        {
            var currentKhach = await _context.KhachThues.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (currentKhach == null)
            {
                return false;
            }

            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(hinhAnh.FileName);
                string path = Path.Combine(_hostEnvironment.WebRootPath, "images", "khachthue");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(stream);
                }

                model.HinhAnh = fileName;
            }
            else
            {
                model.HinhAnh = currentKhach.HinhAnh;
            }

            _context.Update(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.KhachThues.FindAsync(id);
            if (item != null)
            {
                _context.KhachThues.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
