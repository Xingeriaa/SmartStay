using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace do_an_tot_nghiep.Controllers.Api
{
    [ApiController]
    [Route("api/seed")]
    public class SeedApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeedApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("generate-mock-dashboard")]
        public async Task<IActionResult> GenerateMockData()
        {
            try
            {
                // 1. Tạo 1 Tòa Nhà (NhaTro)
                var building = await _context.NhaTros.FirstOrDefaultAsync(n => n.TenNhaTro == "SmartStay Building Alpha");
                if (building == null)
                {
                    building = new NhaTro { TenNhaTro = "SmartStay Building Alpha", DiaChiChiTiet = "123 Đường Tôn Đức Thắng, Quận 1", ManagerId = 1, TotalFloors = 5 };
                    _context.NhaTros.Add(building);
                    await _context.SaveChangesAsync();
                }

                // 2. Tạo Phòng Trọ (Rooms)
                var rooms = new List<PhongTro>();
                for (int i = 1; i <= 5; i++)
                {
                    var rCode = $"10{i}";
                    var existingRm = await _context.PhongTros.FirstOrDefaultAsync(r => r.TenPhong == rCode);
                    if (existingRm == null)
                    {
                        var status = i <= 3 ? (byte)2 : (byte)1; // 3 Occupied, 2 Vacant
                        existingRm = new PhongTro
                        {
                            TenPhong = rCode,
                            NhaTroId = building.Id,
                            GiaPhong = 3500000 + (i * 100000),
                            DienTich = 25,
                            SoLuongNguoi = 2,
                            Tang = 1,
                            Status = status,
                            Balcony = true
                        };
                        _context.PhongTros.Add(existingRm);
                        await _context.SaveChangesAsync();
                    }
                    rooms.Add(existingRm);
                }

                int invoicesAdded = 0;
                int tenantsAdded = 0;

                // 3. Tạo Khách Thuê & Hợp Đồng cho các phòng Occupied
                var currentMonth = DateTime.Now;
                var random = new Random();

                foreach (var room in rooms.Where(x => x.Status == 2)) // Chỉ phòng có người ở
                {
                    bool hasContract = await _context.HopDongs.AnyAsync(h => h.PhongTroId == room.Id);
                    if (hasContract) continue;

                    var tenantRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Tenant");
                    if (tenantRole == null)
                    {
                        tenantRole = new Role { RoleName = "Tenant", Description = "Khách thuê" };
                        _context.Roles.Add(tenantRole);
                        await _context.SaveChangesAsync();
                    }

                    var user = new User
                    {
                        Username = "tenant_" + room.TenPhong + random.Next(100, 999),
                        Password = "pass",
                        FullName = "Cư Dân Phòng " + room.TenPhong,
                        RoleId = tenantRole.Id,
                        IsActive = true
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var tenant = new KhachThue
                    {
                        UserId = user.Id,
                        HoTen = "Cư Dân Phòng " + room.TenPhong,
                        SoDienThoai1 = "0909" + random.Next(100000, 999999).ToString(),
                        Gender = "Nam",
                        SoCCCD = random.Next(10000000, 99999999).ToString()
                    };
                    _context.KhachThues.Add(tenant);
                    await _context.SaveChangesAsync();
                    tenantsAdded++;

                    var contract = new HopDong
                    {
                        ContractCode = $"HD-{DateTime.UtcNow.Ticks}-{room.TenPhong}",
                        PhongTroId = room.Id,
                        NgayBatDau = currentMonth.AddMonths(-6),
                        NgayKetThuc = currentMonth.AddMonths(6),
                        PaymentCycle = 1,
                        TienCoc = room.GiaPhong,
                        GiaThue = room.GiaPhong,
                        TrangThai = TrangThaiHopDong.DangHieuLuc
                    };
                    _context.HopDongs.Add(contract);
                    await _context.SaveChangesAsync();

                    _context.HopDongKhachThues.Add(new HopDongKhachThue { HopDongId = contract.Id, KhachThueId = tenant.Id, IsRepresentative = true });
                    await _context.SaveChangesAsync();

                    // 4. Tạo Hóa đơn và Đồng hồ điện nước 6 tháng qua cho hợp đồng này
                    for (int m = 5; m >= 0; m--)
                    {
                        var targetDate = currentMonth.AddMonths(-m);
                        var myStr = targetDate.ToString("yyyy-MM");

                        // Chỉ số điện nước giả lập
                        var oldE = random.Next(1000, 5000);
                        var newE = oldE + random.Next(50, 150);
                        var oldW = random.Next(100, 500);
                        var newW = oldW + random.Next(5, 15);

                        _context.MeterReadings.Add(new MeterReading
                        {
                            RoomId = room.Id,
                            MonthYear = myStr,
                            OldElectricityIndex = oldE,
                            NewElectricityIndex = newE,
                            OldWaterIndex = oldW,
                            NewWaterIndex = newW
                        });
                        await _context.SaveChangesAsync();

                        // Giả lập trạng thái hóa đơn
                        string invStatus = "Paid";
                        if (m == 0 && random.Next(0, 100) > 40) invStatus = "Unpaid";
                        if (m == 1 && random.Next(0, 100) > 80) invStatus = "Overdue";

                        decimal invoiceTotal = contract.GiaThue + ((newE - oldE) * 3500) + ((newW - oldW) * 20000);

                        var invoice = new Invoice
                        {
                            ContractId = contract.Id,
                            InvoiceCode = $"INV-{myStr}-{room.TenPhong}",
                            Period = "Từ 01 đến 30",
                            MonthYear = myStr,
                            SubTotal = invoiceTotal,
                            TotalAmount = invoiceTotal,
                            Status = invStatus,
                            DueDate = targetDate.AddDays(5),
                            PaymentDate = invStatus == "Paid" ? targetDate.AddDays(2) : null
                        };
                        _context.Invoices.Add(invoice);
                        await _context.SaveChangesAsync();
                        invoicesAdded++;

                        // Giả lập Ledger nợ xấu
                        if (invStatus == "Overdue")
                        {
                            var tb = await _context.TenantBalances.FirstOrDefaultAsync(b => b.TenantId == tenant.Id);
                            if (tb == null)
                            {
                                tb = new TenantBalance { TenantId = tenant.Id, BalanceAmount = -invoiceTotal, LastUpdatedAt = DateTime.UtcNow };
                                _context.TenantBalances.Add(tb);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }

                return Ok(new
                {
                    message = "🎉 Bơm dữ liệu giả lập thành công! Hãy Refresh lại Dashboard.",
                    debug = new
                    {
                        rooms_count = rooms.Count,
                        rooms_occupied = rooms.Count(x => x.Status == 2),
                        tenants_added = tenantsAdded,
                        invoices_added = invoicesAdded
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }
    }
}
