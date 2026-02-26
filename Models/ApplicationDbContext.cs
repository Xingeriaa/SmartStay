#pragma warning disable CS8618
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<DichVu> DichVu { get; set; }
        public DbSet<ServicePriceHistory> ServicePriceHistory { get; set; }
        public DbSet<NhaTro> NhaTros { get; set; }
        public DbSet<PhongTro> PhongTros { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<KhachThue> KhachThues { get; set; }
        public DbSet<HopDongKhachThue> HopDongKhachThues { get; set; }
        public DbSet<Liquidation> Liquidations { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<RoomAsset> RoomAssets { get; set; }
        public DbSet<ContractExtension> ContractExtensions { get; set; }
        public DbSet<ContractService> ContractServices { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketImage> TicketImages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RoomStatusHistory> RoomStatusHistories { get; set; } = null!;
        public DbSet<TenantBalance> TenantBalances { get; set; } = null!;
        public DbSet<TenantBalanceTransaction> TenantBalanceTransactions { get; set; } = null!;
        public DbSet<PaymentWebhookLog> PaymentWebhookLogs { get; set; } = null!;
        public DbSet<AssetMaintenanceLog> AssetMaintenanceLogs { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<RoomAnalyticsSnapshot> RoomAnalyticsSnapshots { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<PhongTro>()
                .HasIndex(r => new { r.NhaTroId, r.TenPhong })
                .IsUnique();

            modelBuilder.Entity<HopDong>()
                .HasIndex(c => c.ContractCode)
                .IsUnique();

            modelBuilder.Entity<MeterReading>()
                .HasIndex(m => new { m.RoomId, m.MonthYear })
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceCode)
                .IsUnique();

            modelBuilder.Entity<PaymentTransaction>()
                .HasIndex(p => p.TransactionCode)
                .IsUnique();

            modelBuilder.Entity<KhachThue>()
                .HasIndex(t => t.SoCCCD)
                .IsUnique();

            modelBuilder.Entity<HopDongKhachThue>()
                .HasOne(x => x.HopDong)
                .WithMany(x => x.HopDongKhachThues)
                .HasForeignKey(x => x.HopDongId);

            modelBuilder.Entity<HopDongKhachThue>()
                .HasOne(x => x.KhachThue)
                .WithMany(x => x.HopDongKhachThues)
                .HasForeignKey(x => x.KhachThueId);

            modelBuilder.Entity<HopDong>()
                .Property(x => x.TrangThai)
                .HasColumnName("ContractStatus")
                .HasConversion(
                    v => ToContractStatus(v),
                    v => FromContractStatus(v));

            modelBuilder.Entity<Liquidation>()
                .HasOne(x => x.Contract)
                .WithOne(x => x.Liquidation)
                .HasForeignKey<Liquidation>(x => x.ContractId);

            modelBuilder.Entity<InvoiceDetail>()
                .HasOne<Invoice>()
                .WithMany(i => i.Details)
                .HasForeignKey(d => d.InvoiceId);

            modelBuilder.Entity<TicketImage>()
                .HasOne<Ticket>()
                .WithMany(t => t.Images)
                .HasForeignKey(i => i.TicketId);

            modelBuilder.Entity<Image>()
                .ToTable("Images", t => t.HasCheckConstraint("CK_Images_OnlyOneFK",
                "([BuildingId] IS NOT NULL AND [RoomId] IS NULL AND [AssetId] IS NULL) OR " +
                "([BuildingId] IS NULL AND [RoomId] IS NOT NULL AND [AssetId] IS NULL) OR " +
                "([BuildingId] IS NULL AND [RoomId] IS NULL AND [AssetId] IS NOT NULL)"));
        }

        private static string ToContractStatus(TrangThaiHopDong status)
        {
            if (status == TrangThaiHopDong.DangHieuLuc)
            {
                return "Active";
            }
            if (status == TrangThaiHopDong.DaThanhLy)
            {
                return "Terminated";
            }
            if (status == TrangThaiHopDong.Huy)
            {
                return "Terminated";
            }

            return "Draft";
        }

        private static TrangThaiHopDong FromContractStatus(string status)
        {
            if (status == "Active")
            {
                return TrangThaiHopDong.DangHieuLuc;
            }
            if (status == "Terminated")
            {
                return TrangThaiHopDong.DaThanhLy;
            }
            if (status == "Expired")
            {
                return TrangThaiHopDong.Huy;
            }

            return TrangThaiHopDong.DangHieuLuc;
        }
    }
}
