using do_an_tot_nghiep.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý hợp đồng thuê phòng.
    /// </summary>
    [ApiController]
    [Route("api/hopdong")]
    public class HopDongApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HopDongApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách hợp đồng.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.HopDongs
                .Where(h => !h.IsDeleted)
                .Include(h => h.PhongTro)
                .ToListAsync();

            var result = list.Select(h => new
            {
                id = h.Id,
                contractCode = h.ContractCode,
                phongTroId = h.PhongTroId,
                tenPhong = h.PhongTro?.TenPhong,
                ngayBatDau = h.NgayBatDau,
                ngayKetThuc = h.NgayKetThuc,
                trangThai = h.TrangThai switch
                {
                    TrangThaiHopDong.DangHieuLuc => "Active",
                    TrangThaiHopDong.DaThanhLy => "Terminated",
                    TrangThaiHopDong.Huy => "Cancelled",
                    _ => "Draft"
                },
                tienCoc = h.TienCoc,
                giaThue = h.GiaThue,
                paymentCycle = h.PaymentCycle,
                signatureDate = h.SignatureDate
            });

            return Ok(result);
        }

        /// <summary>
        /// Lấy hợp đồng theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var h = await _context.HopDongs
                .Include(x => x.PhongTro)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (h == null) return NotFound();

            return Ok(new
            {
                id = h.Id,
                contractCode = h.ContractCode,
                phongTroId = h.PhongTroId,
                tenPhong = h.PhongTro?.TenPhong,
                ngayBatDau = h.NgayBatDau,
                ngayKetThuc = h.NgayKetThuc,
                signatureDate = h.SignatureDate,
                paymentCycle = h.PaymentCycle,
                giaThue = h.GiaThue,
                tienCoc = h.TienCoc,
                depositStatus = h.DepositStatus,
                trangThai = h.TrangThai switch
                {
                    TrangThaiHopDong.DangHieuLuc => "Active",
                    TrangThaiHopDong.DaThanhLy => "Terminated",
                    TrangThaiHopDong.Huy => "Cancelled",
                    _ => "Draft"
                },
                isDeleted = h.IsDeleted,
                createdAt = h.CreatedAt
            });
        }

        /// <summary>
        /// Tạo hợp đồng mới.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContractRequest req) // using a DTO
        {
            // Verify room is available
            var room = await _context.PhongTros.FindAsync(req.PhongTroId);
            if (room == null || room.IsDeleted) return NotFound(new { message = "Không tìm thấy phòng" });
            
            // Check for existing Active contract
            bool hasActive = await _context.HopDongs
                .AnyAsync(h => h.PhongTroId == req.PhongTroId && h.TrangThai == TrangThaiHopDong.DangHieuLuc && !h.IsDeleted);
                
            if (hasActive) return BadRequest("Phòng đã có hợp đồng Active, không thể tạo mới.");
            
            if (room.Status != 1) return BadRequest("Phòng phải ở trạng thái Trống (Vacant) mới được tạo hợp đồng.");

            var hopDong = new HopDong
            {
               ContractCode = $"HD-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
               PhongTroId = req.PhongTroId,
               NgayBatDau = req.NgayBatDau,
               NgayKetThuc = req.NgayKetThuc,
               PaymentCycle = req.PaymentCycle > 0 ? req.PaymentCycle : 1,
               TienCoc = req.TienCoc,
               GiaThue = room.GiaPhong, // Snapshot current price
               SignatureDate = DateTime.Today,
               DepositStatus = "Available",
               TrangThai = TrangThaiHopDong.DangHieuLuc, // Active
               CreatedAt = DateTime.UtcNow
            };
            
            _context.HopDongs.Add(hopDong);
            
            // Add Tenants
            if (req.KhachThueIds != null && req.KhachThueIds.Any())
            {
               for(int i = 0; i < req.KhachThueIds.Count; i++)
               {
                   _context.HopDongKhachThues.Add(new HopDongKhachThue
                   {
                       HopDong = hopDong,
                       KhachThueId = req.KhachThueIds[i],
                       IsRepresentative = (i == 0) // First is owner
                   });
               }
            }

            // TRIGGER STATE MACHINE: Vacant -> Occupied
            byte oldStatus = room.Status;
            room.Status = 2; // Occupied
            
            _context.RoomStatusHistories.Add(new RoomStatusHistory
            {
                PhongTro = room,
                OldStatus = oldStatus,
                NewStatus = 2,
                ChangedAt = DateTime.UtcNow,
                Reason = $"Ký hợp đồng {hopDong.ContractCode}",
                AutoGenerated = true
            });
            
            _context.Entry(room).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok(hopDong);
        }

        /// <summary>
        /// Cập nhật hợp đồng.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, HopDong hopDong)
        {
            if (id != hopDong.Id) return BadRequest();

            _context.Entry(hopDong).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa hợp đồng. (Soft Delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null) return NotFound();
            
            if (hopDong.TrangThai == TrangThaiHopDong.DangHieuLuc)
            {
                return BadRequest(new { message = "Không thể xóa hợp đồng đang Active. Phải tiến hành Thanh Lý." });
            }

            hopDong.IsDeleted = true;
            _context.Entry(hopDong).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
    
    public class CreateContractRequest
    {
        public int PhongTroId { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int PaymentCycle { get; set; }
        public decimal TienCoc { get; set; }
        public List<int>? KhachThueIds { get; set; }
    }
}
