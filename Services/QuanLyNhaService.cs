using do_an_tot_nghiep.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace do_an_tot_nghiep.Services
{
    public interface IQuanLyNhaService
    {
        Task<QuanLyNhaIndexData> GetIndexDataAsync();
        Task<List<DichVu>> GetDichVuListAsync();
        Task CreateAsync(NhaTro model, List<string> dichVuSelect);
        Task<DeleteSelectedResult> DeleteSelectedAsync(List<int> ids);
    }

    public sealed record QuanLyNhaIndexData(List<NhaTro> NhaTros, List<DichVu> AllDichVus);

    public class QuanLyNhaService : IQuanLyNhaService
    {
        private readonly ApplicationDbContext _context;

        public QuanLyNhaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QuanLyNhaIndexData> GetIndexDataAsync()
        {
            var allDichVus = await _context.DichVu.ToListAsync();
            var listNha = await _context.NhaTros.ToListAsync();
            return new QuanLyNhaIndexData(listNha, allDichVus);
        }

        public Task<List<DichVu>> GetDichVuListAsync()
        {
            return _context.DichVu.ToListAsync();
        }

        public async Task CreateAsync(NhaTro model, List<string> dichVuSelect)
        {
            model.DanhSachDichVu = dichVuSelect != null ? string.Join(",", dichVuSelect) : string.Empty;
            model.DiaChiChiTiet = BuildAddress(model);

            _context.NhaTros.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task<DeleteSelectedResult> DeleteSelectedAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new DeleteSelectedResult(false, "Không có mục nào được chọn.");
            }

            try
            {
                var listToDelete = await _context.NhaTros.Where(x => ids.Contains(x.Id)).ToListAsync();

                if (listToDelete.Any())
                {
                    _context.NhaTros.RemoveRange(listToDelete);
                    await _context.SaveChangesAsync();
                    return new DeleteSelectedResult(true, $"Đã xóa thành công {listToDelete.Count} mục.");
                }

                return new DeleteSelectedResult(false, "Dữ liệu không tồn tại.");
            }
            catch (Exception ex)
            {
                return new DeleteSelectedResult(false, "Lỗi hệ thống: " + ex.Message);
            }
        }

        private static string BuildAddress(NhaTro model)
        {
            var parts = new List<string>();
            AddPart(parts, model.DiaChiChiTiet);
            AddPart(parts, model.PhuongXa);
            AddPart(parts, model.QuanHuyen);
            AddPart(parts, model.TinhThanh);

            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private static void AddPart(List<string> parts, string? value)
        {
            var trimmed = value?.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                parts.Add(trimmed);
            }
        }
    }
}
