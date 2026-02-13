using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Services
{
    public interface IHopDongService
    {
        Task<List<HopDong>> GetAllAsync();
        Task<HopDong?> GetDetailsAsync(int id);
        HopDongFormViewModel BuildCreateFormViewModel(List<PhongTro> phongTros, List<KhachThue> khachThues);
        Task<HopDongFormViewModel> BuildCreateFormAsync();
        Task<List<(string Key, string Message)>> ValidateCreateAsync(HopDongFormViewModel vm);
        Task PopulateCreateListsAsync(HopDongFormViewModel vm);
        Task CreateAsync(HopDongFormViewModel vm);
        Task<HopDong?> GetForEditAsync(int id);
        Task<bool> UpdateAsync(int id, HopDong model);
        Task<HopDong?> GetForExtendAsync(int id);
        Task<ExtendResult> ExtendAsync(int id, DateTime? ngayKetThucMoi);
        Task<HopDong?> GetForTerminateAsync(int id);
        Task<TerminateResult> TerminateAsync(int id, HopDong model, int staffId);
        void MapLiquidationToViewFields(HopDong hopDong);
        Task<int> ResolveUserIdAsync(string? username);
    }

    public sealed record ExtendResult(HopDong? HopDong, string? ErrorMessage);

    public sealed record TerminateResult(bool Success, int HopDongId);

    public class HopDongService : IHopDongService
    {
        private readonly ApplicationDbContext _context;

        public HopDongService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<HopDong>> GetAllAsync()
        {
            return _context.HopDongs
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.NhaTro)
                .Include(h => h.HopDongKhachThues)
                    .ThenInclude(hk => hk.KhachThue)
                .Include(h => h.Liquidation)
                .OrderByDescending(h => h.NgayBatDau)
                .ToListAsync();
        }

        public Task<HopDong?> GetDetailsAsync(int id)
        {
            return _context.HopDongs
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.NhaTro)
                .Include(h => h.HopDongKhachThues)
                    .ThenInclude(hk => hk.KhachThue)
                .Include(h => h.Liquidation)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public HopDongFormViewModel BuildCreateFormViewModel(List<PhongTro> phongTros, List<KhachThue> khachThues)
        {
            return new HopDongFormViewModel
            {
                PhongTros = phongTros,
                KhachThues = khachThues,
                HopDong = new HopDong
                {
                    NgayBatDau = DateTime.Today,
                    TrangThai = TrangThaiHopDong.DangHieuLuc,
                    SignatureDate = DateTime.Today
                }
            };
        }

        public async Task<HopDongFormViewModel> BuildCreateFormAsync()
        {
            var phongTros = await _context.PhongTros
                .Include(p => p.NhaTro)
                .OrderBy(p => p.NhaTro!.TenNhaTro)
                .ThenBy(p => p.TenPhong)
                .ToListAsync();

            var khachThues = await _context.KhachThues
                .OrderBy(k => k.HoTen)
                .ToListAsync();

            return BuildCreateFormViewModel(phongTros, khachThues);
        }

        public async Task<List<(string Key, string Message)>> ValidateCreateAsync(HopDongFormViewModel vm)
        {
            var errors = new List<(string Key, string Message)>();

            if (vm.HopDong.PhongTroId == 0)
            {
                errors.Add(("HopDong.PhongTroId", "Vui lòng chọn phòng."));
            }

            if (vm.ChuHopDongId == null)
            {
                errors.Add((nameof(vm.ChuHopDongId), "Vui lòng chọn chủ hợp đồng."));
            }

            if (!vm.HopDong.NgayKetThuc.HasValue)
            {
                errors.Add(("HopDong.NgayKetThuc", "Vui lòng nhập ngày kết thúc."));
            }
            else if (vm.HopDong.NgayKetThuc <= vm.HopDong.NgayBatDau)
            {
                errors.Add(("HopDong.NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu."));
            }

            if (vm.HopDong.PhongTroId != 0)
            {
                var phongDangThue = await _context.HopDongs
                    .AnyAsync(h => h.PhongTroId == vm.HopDong.PhongTroId && h.TrangThai == TrangThaiHopDong.DangHieuLuc);

                if (phongDangThue)
                {
                    errors.Add(("HopDong.PhongTroId", "Phòng này đang có hợp đồng hiệu lực."));
                }
            }

            return errors;
        }

        public async Task PopulateCreateListsAsync(HopDongFormViewModel vm)
        {
            vm.PhongTros = await _context.PhongTros
                .Include(p => p.NhaTro)
                .OrderBy(p => p.NhaTro!.TenNhaTro)
                .ThenBy(p => p.TenPhong)
                .ToListAsync();

            vm.KhachThues = await _context.KhachThues
                .OrderBy(k => k.HoTen)
                .ToListAsync();
        }

        public async Task CreateAsync(HopDongFormViewModel vm)
        {
            if (vm.HopDong.TrangThai == 0)
            {
                vm.HopDong.TrangThai = TrangThaiHopDong.DangHieuLuc;
            }

            vm.HopDong.ContractCode = GenerateContractCode();
            vm.HopDong.SignatureDate = DateTime.Today;
            vm.HopDong.PaymentCycle = vm.HopDong.PaymentCycle <= 0 ? 1 : vm.HopDong.PaymentCycle;
            vm.HopDong.DepositStatus = string.IsNullOrWhiteSpace(vm.HopDong.DepositStatus) ? "Available" : vm.HopDong.DepositStatus;

            _context.HopDongs.Add(vm.HopDong);
            await _context.SaveChangesAsync();

            var chuHopDongId = vm.ChuHopDongId!.Value;
            var hopDongKhachThues = new List<HopDongKhachThue>
            {
                new HopDongKhachThue
                {
                    HopDongId = vm.HopDong.Id,
                    KhachThueId = chuHopDongId,
                    IsRepresentative = true
                }
            };

            var cuDanIds = vm.CuDanIds
                .Where(id => id != chuHopDongId)
                .Distinct()
                .ToList();

            foreach (var id in cuDanIds)
            {
                hopDongKhachThues.Add(new HopDongKhachThue
                {
                    HopDongId = vm.HopDong.Id,
                    KhachThueId = id,
                    IsRepresentative = false
                });
            }

            _context.HopDongKhachThues.AddRange(hopDongKhachThues);
            await _context.SaveChangesAsync();
        }

        public Task<HopDong?> GetForEditAsync(int id)
        {
            return _context.HopDongs
                .Include(h => h.PhongTro)
                    .ThenInclude(p => p.NhaTro)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<bool> UpdateAsync(int id, HopDong model)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null)
            {
                return false;
            }

            hopDong.NgayBatDau = model.NgayBatDau;
            hopDong.NgayKetThuc = model.NgayKetThuc;
            hopDong.GiaThue = model.GiaThue;
            hopDong.TienCoc = model.TienCoc;
            hopDong.TrangThai = model.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public Task<HopDong?> GetForExtendAsync(int id)
        {
            return _context.HopDongs
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<ExtendResult> ExtendAsync(int id, DateTime? ngayKetThucMoi)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null)
            {
                return new ExtendResult(null, null);
            }

            if (!ngayKetThucMoi.HasValue || ngayKetThucMoi <= hopDong.NgayBatDau)
            {
                return new ExtendResult(hopDong, "Ngày kết thúc mới phải sau ngày bắt đầu.");
            }

            hopDong.NgayKetThuc = ngayKetThucMoi;
            hopDong.TrangThai = TrangThaiHopDong.DangHieuLuc;
            await _context.SaveChangesAsync();

            return new ExtendResult(hopDong, null);
        }

        public Task<HopDong?> GetForTerminateAsync(int id)
        {
            return _context.HopDongs
                .Include(h => h.PhongTro)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<TerminateResult> TerminateAsync(int id, HopDong model, int staffId)
        {
            var hopDong = await _context.HopDongs
                .Include(h => h.Liquidation)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hopDong == null)
            {
                return new TerminateResult(false, 0);
            }

            hopDong.TrangThai = TrangThaiHopDong.DaThanhLy;

            var liquidation = hopDong.Liquidation ?? new Liquidation
            {
                ContractId = hopDong.Id,
                StaffId = staffId
            };

            var latestInvoiceId = await _context.Invoices
                .Where(x => x.ContractId == hopDong.Id)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (latestInvoiceId == 0)
            {
                var now = DateTime.UtcNow;
                var monthYear = now.ToString("yyyy-MM");
                var fallbackInvoice = new Invoice
                {
                    InvoiceCode = $"INV-LQD-{now:yyyyMMddHHmmssfff}",
                    ContractId = hopDong.Id,
                    Period = monthYear,
                    MonthYear = monthYear,
                    SubTotal = 0,
                    TaxAmount = 0,
                    TotalAmount = 0,
                    DueDate = now,
                    Status = "Paid",
                    CreatedAt = now
                };

                _context.Invoices.Add(fallbackInvoice);
                await _context.SaveChangesAsync();
                latestInvoiceId = fallbackInvoice.Id;
            }

            liquidation.CreatedAt = model.NgayThanhLy ?? DateTime.Today;
            liquidation.InvoiceId = latestInvoiceId;
            liquidation.FinalInvoiceAmount = model.TongTienQuyetToan ?? 0;
            liquidation.RefundAmount = model.SoTienHoanTra ?? 0;
            liquidation.DepositUsed = 0;
            liquidation.AdditionalCharge = 0;
            liquidation.Reason = model.GhiChuThanhLy;

            if (hopDong.Liquidation == null)
            {
                _context.Liquidations.Add(liquidation);
            }

            await _context.SaveChangesAsync();
            return new TerminateResult(true, hopDong.Id);
        }

        public void MapLiquidationToViewFields(HopDong hopDong)
        {
            if (hopDong.Liquidation == null)
            {
                return;
            }

            hopDong.NgayThanhLy = hopDong.Liquidation.CreatedAt;
            hopDong.TongTienQuyetToan = hopDong.Liquidation.FinalInvoiceAmount;
            hopDong.SoTienHoanTra = hopDong.Liquidation.RefundAmount;
            hopDong.GhiChuThanhLy = hopDong.Liquidation.Reason;
        }

        public async Task<int> ResolveUserIdAsync(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                var fallback = await _context.Users.OrderBy(u => u.Id).FirstOrDefaultAsync();
                return fallback?.Id ?? 1;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                var fallback = await _context.Users.OrderBy(u => u.Id).FirstOrDefaultAsync();
                return fallback?.Id ?? 1;
            }

            return user.Id;
        }

        private static string GenerateContractCode()
        {
            return $"HD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}
