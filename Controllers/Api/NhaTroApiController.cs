using do_an_tot_nghiep.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý tòa nhà/nhà trọ.
    /// </summary>
    [ApiController]
    [Route("api/nhatro")]
    public class NhaTroApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NhaTroApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách nhà trọ.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<NhaTro>>> GetAll()
        {
            var buildings = await _context.NhaTros.Where(n => !n.IsDeleted).ToListAsync();
            var bIds = buildings.Select(b => b.Id).ToList();
            var thumbs = await _context.Images
                .Where(i => i.BuildingId != null && bIds.Contains(i.BuildingId.Value) && i.IsThumbnail)
                .ToListAsync();

            foreach (var b in buildings)
            {
                var t = thumbs.FirstOrDefault(x => x.BuildingId == b.Id);
                if (t != null) b.AnhNhaTro = t.ImageUrl;
            }
            return buildings;
        }

        /// <summary>
        /// Lấy nhà trọ theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<NhaTro>> GetById(int id)
        {
            var nha = await _context.NhaTros.FindAsync(id);
            if (nha == null || nha.IsDeleted) return NotFound();

            var thumb = await _context.Images.FirstOrDefaultAsync(i => i.BuildingId == id && i.IsThumbnail);
            if (thumb != null) nha.AnhNhaTro = thumb.ImageUrl;

            return nha;
        }

        /// <summary>
        /// Tạo nhà trọ mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<NhaTro>> Create(NhaTro nha)
        {
            nha.DiaChiChiTiet = BuildAddress(nha);
            _context.NhaTros.Add(nha);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = nha.Id }, nha);
        }

        /// <summary>
        /// Cập nhật nhà trọ.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, NhaTro model)
        {
            if (id != model.Id) return BadRequest();

            var nha = await _context.NhaTros.FindAsync(id);
            if (nha == null || nha.IsDeleted) return NotFound();

            nha.TenNhaTro = model.TenNhaTro;
            nha.LoaiNha = model.LoaiNha;
            nha.GiaThue = model.GiaThue;
            nha.NgayThuTien = model.NgayThuTien;

            // Address logic
            nha.DiaChiChiTiet = BuildAddress(model);

            nha.DanhSachDichVu = model.DanhSachDichVu;
            nha.GhiChu = model.GhiChu; // triggers the logic UpdateDescription
            nha.OperationDate = model.OperationDate;

            // Safe zone new fields
            nha.TotalFloors = model.TotalFloors;
            nha.Latitude = model.Latitude;
            nha.Longitude = model.Longitude;
            nha.ElectricityProvider = model.ElectricityProvider;
            nha.WaterProvider = model.WaterProvider;
            nha.FireSafetyCertificateExpiry = model.FireSafetyCertificateExpiry;
            nha.LastMaintenanceDate = model.LastMaintenanceDate;
            nha.ManagerId = model.ManagerId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhaTroExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        private bool NhaTroExists(int id)
        {
            return _context.NhaTros.Any(e => e.Id == id);
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

        /// <summary>
        /// Xóa nhà trọ.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var nha = await _context.NhaTros.FindAsync(id);

            if (nha == null || nha.IsDeleted) return NotFound();

            var roomIds = await _context.PhongTros
                .Where(p => p.NhaTroId == id && !p.IsDeleted)
                .Select(p => p.Id)
                .ToListAsync();

            bool hasActiveContract = await _context.HopDongs
                .AnyAsync(h => roomIds.Contains(h.PhongTroId) && h.TrangThai == TrangThaiHopDong.DangHieuLuc);

            if (hasActiveContract)
                return BadRequest(new { message = "Không thể xóa tòa nhà vì còn hợp đồng Active." });

            bool hasAssets = await _context.RoomAssets
                .AnyAsync(ra => roomIds.Contains(ra.RoomId));

            if (hasAssets)
                return BadRequest(new { message = "Không thể xóa tòa nhà vì còn tài sản chưa thanh lý." });

            nha.IsDeleted = true;
            _context.Entry(nha).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
