using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Services
{
    public interface IPhongTroService
    {
        Task<List<NhaTro>> GetAllNhaAsync();
        Task<List<PhongTro>> GetPhongByNhaAsync(int? nhaId);
        Task<NhaTro?> GetNhaAsync(int nhaId);
        Task<PhongTroCreateData?> GetCreateDataAsync(int? nhaId);
        Task<PhongTroEditData?> GetEditDataAsync(int id);
        Task<PhongTroDetailsData?> GetDetailsAsync(int id);
        Task CreateAsync(PhongTro phong);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(PhongTro model);
        Task<DeleteMultipleResult> DeleteMultipleAsync(List<int> ids);
    }

    public sealed record PhongTroCreateData(NhaTro NhaTro, List<DichVu> DichVuHienThi, PhongTro Model);

    public sealed record PhongTroEditData(PhongTro PhongTro, NhaTro? NhaTro, List<string> DichVuCuaNha);

    public sealed record PhongTroDetailsData(PhongTro PhongTro, List<string> DichVuPhong, string TrangThaiPhong);

    public sealed record DeleteMultipleResult(bool Success, string? Message, List<string>? Rooms);

    public class PhongTroService : IPhongTroService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PhongTroService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public Task<List<NhaTro>> GetAllNhaAsync()
        {
            return _context.NhaTros.Where(n => !n.IsDeleted).ToListAsync();
        }

        public async Task<List<PhongTro>> GetPhongByNhaAsync(int? nhaId)
        {
            if (!nhaId.HasValue)
            {
                return new List<PhongTro>();
            }

            var dsPhong = await _context.PhongTros
                .Where(p => p.NhaTroId == nhaId && !p.IsDeleted)
                .ToListAsync();

            var roomIds = dsPhong.Select(r => r.Id).ToList();
            var thumbs = await _context.Images
                .Where(i => i.RoomId != null && roomIds.Contains(i.RoomId.Value) && i.IsThumbnail)
                .ToListAsync();

            foreach (var p in dsPhong)
            {
                var t = thumbs.FirstOrDefault(x => x.RoomId == p.Id);
                if (t != null) p.AnhPhong = t.ImageUrl;
            }

            return dsPhong;
        }

        public Task<NhaTro?> GetNhaAsync(int nhaId)
        {
            return _context.NhaTros.FirstOrDefaultAsync(n => n.Id == nhaId && !n.IsDeleted);
        }

        public async Task<PhongTroCreateData?> GetCreateDataAsync(int? nhaId)
        {
            if (!nhaId.HasValue)
            {
                return null;
            }

            var nha = await _context.NhaTros.FirstOrDefaultAsync(n => n.Id == nhaId.Value && !n.IsDeleted);
            if (nha == null)
            {
                return null;
            }

            var serviceIds = nha.DanhSachDichVu?
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>();

            var allDichVu = await _context.DichVu.ToListAsync();
            var dichVuHienThi = allDichVu
                .Where(d => serviceIds.Contains(d.Id.ToString()))
                .OrderBy(d => d.Ten)
                .ToList();

            var model = new PhongTro
            {
                NhaTroId = nha.Id,
                GiaPhong = nha.GiaThue
            };

            return new PhongTroCreateData(nha, dichVuHienThi, model);
        }

        public async Task<PhongTroEditData?> GetEditDataAsync(int id)
        {
            var phong = await _context.PhongTros
                .Include(p => p.NhaTro)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (phong == null)
            {
                return null;
            }

            var thumb = await _context.Images.FirstOrDefaultAsync(i => i.RoomId == phong.Id && i.IsThumbnail);
            if (thumb != null) phong.AnhPhong = thumb.ImageUrl;

            var nha = phong.NhaTro;
            var serviceIds = nha?.DanhSachDichVu?
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>();

            var allDichVu = await _context.DichVu.ToListAsync();
            var dichVuCuaNha = allDichVu
                .Where(d => serviceIds.Contains(d.Id.ToString()))
                .Select(d => d.Ten)
                .ToList();

            return new PhongTroEditData(phong, nha, dichVuCuaNha);
        }

        public async Task<PhongTroDetailsData?> GetDetailsAsync(int id)
        {
            var phong = await _context.PhongTros
                .Include(p => p.NhaTro)
                .Include(p => p.HopDongs)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (phong == null)
            {
                return null;
            }

            var thumb = await _context.Images.FirstOrDefaultAsync(i => i.RoomId == phong.Id && i.IsThumbnail);
            if (thumb != null) phong.AnhPhong = thumb.ImageUrl;

            var trangThai = phong.HopDongs != null && phong.HopDongs.Any()
                ? "Đang có người thuê"
                : "Phòng trống";

            var serviceIds = phong.NhaTro?.DanhSachDichVu?
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>();

            var allDichVu = await _context.DichVu.ToListAsync();
            var dichVuPhong = allDichVu
                .Where(d => serviceIds.Contains(d.Id.ToString()))
                .Select(d => d.Ten)
                .ToList();

            return new PhongTroDetailsData(phong, dichVuPhong, trangThai);
        }

        public async Task CreateAsync(PhongTro phong)
        {
            if (string.IsNullOrWhiteSpace(phong.DoiTuong))
            {
                phong.DoiTuong = "Standard";
            }

            if (phong.ImageFile != null)
            {
                string folder = "images/phongtro/";
                string fileName = Guid.NewGuid() + "_" + phong.ImageFile.FileName;
                string path = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                await phong.ImageFile.CopyToAsync(stream);

                phong.AnhPhong = "/" + folder + fileName;
            }

            phong.Status = 1; // Default to Vacant
            _context.PhongTros.Add(phong);

            // Log initial state
            _context.RoomStatusHistories.Add(new RoomStatusHistory
            {
                PhongTro = phong,
                OldStatus = 1,
                NewStatus = 1,
                Reason = "Khởi tạo phòng mới",
                AutoGenerated = true
            });

            await _context.SaveChangesAsync();
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(PhongTro model)
        {
            var phong = await _context.PhongTros.FindAsync(model.Id);
            if (phong == null || phong.IsDeleted)
            {
                return (false, "Không tìm thấy phòng.");
            }

            var activeContract = await _context.HopDongs
                .Include(h => h.HopDongKhachThues)
                .FirstOrDefaultAsync(h => h.PhongTroId == model.Id && h.TrangThai == TrangThaiHopDong.DangHieuLuc);

            if (activeContract != null)
            {
                // BR-R2: Không cho sửa RoomCode nếu đã có hợp đồng
                if (phong.TenPhong != model.TenPhong)
                {
                    return (false, "Không thể sửa Mã phòng vì đang có hợp đồng Active.");
                }

                // BR-R3: Không giảm MaxOccupants < số cư dân đang ở
                int currentTenants = activeContract.HopDongKhachThues.Count;
                if (model.SoLuongNguoi < currentTenants)
                {
                    return (false, $"Không thể giảm số lượng người tối đa ({model.SoLuongNguoi}) thấp hơn số người đang ở thực tế ({currentTenants}).");
                }
            }

            phong.TenPhong = model.TenPhong;
            phong.GiaPhong = model.GiaPhong;
            phong.Tang = model.Tang;
            phong.DienTich = model.DienTich;
            phong.SoLuongNguoi = model.SoLuongNguoi;
            phong.TienCoc = model.TienCoc;
            phong.NoiThat = model.NoiThat;
            phong.GhiChu = model.GhiChu;
            phong.DoiTuong = string.IsNullOrWhiteSpace(model.DoiTuong) ? "Standard" : model.DoiTuong;

            if (model.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(phong.AnhPhong))
                {
                    var oldPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        phong.AnhPhong.TrimStart('/'));

                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }

                string folder = "images/phongtro/";
                string fileName = Guid.NewGuid() + "_" + model.ImageFile.FileName;
                string path = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                await model.ImageFile.CopyToAsync(stream);

                phong.AnhPhong = "/" + folder + fileName;
            }

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<DeleteMultipleResult> DeleteMultipleAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new DeleteMultipleResult(false, "Danh sách phòng trống", null);
            }

            var phongs = await _context.PhongTros
                .Include(p => p.HopDongs)
                .Where(p => ids.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync();

            var phongDangThue = phongs
                .Where(p => p.HopDongs.Any(h => h.TrangThai == TrangThaiHopDong.DangHieuLuc))
                .Select(p => p.TenPhong)
                .ToList();

            if (phongDangThue.Any())
            {
                return new DeleteMultipleResult(false, "Không thể xóa phòng đang có hợp đồng Active", phongDangThue);
            }

            var hasAssets = await _context.RoomAssets
                .Where(ra => ids.Contains(ra.RoomId))
                .Select(ra => ra.RoomId)
                .Distinct()
                .ToListAsync();

            if (hasAssets.Any())
            {
                var phongCoTaiSan = phongs.Where(p => hasAssets.Contains(p.Id)).Select(p => p.TenPhong).ToList();
                return new DeleteMultipleResult(false, "Không thể xóa phòng vì còn tài sản chưa thanh lý", phongCoTaiSan);
            }

            foreach (var p in phongs)
            {
                p.IsDeleted = true;
                _context.Entry(p).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();

            return new DeleteMultipleResult(true, null, null);
        }
    }
}
