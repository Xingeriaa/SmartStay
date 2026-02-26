using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace do_an_tot_nghiep.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeDataSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HopDong_PhongTros_PhongTroId",
                table: "HopDong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhongTros_NhaTros_NhaTroId",
                table: "PhongTros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhongTros",
                table: "PhongTros");

            migrationBuilder.DropIndex(
                name: "IX_PhongTros_NhaTroId",
                table: "PhongTros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NhaTros",
                table: "NhaTros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhachThues",
                table: "KhachThues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HopDong",
                table: "HopDong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DichVu",
                table: "DichVu");

            migrationBuilder.DropColumn(
                name: "AnhPhong",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "DichVu",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "TienCoc",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "GiaThue",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "LoaiNha",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "NgayThuTien",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "PhuongXa",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "QuanHuyen",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "TinhThanh",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "NoiCap",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "SoTienCoc",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "GhiChuThanhLy",
                table: "HopDong");

            migrationBuilder.DropColumn(
                name: "NgayThanhLy",
                table: "HopDong");

            migrationBuilder.DropColumn(
                name: "SoTienHoanTra",
                table: "HopDong");

            migrationBuilder.DropColumn(
                name: "TongTienQuyetToan",
                table: "HopDong");

            migrationBuilder.DropColumn(
                name: "DonGia",
                table: "DichVu");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "DichVu");

            migrationBuilder.RenameTable(
                name: "PhongTros",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "NhaTros",
                newName: "Buildings");

            migrationBuilder.RenameTable(
                name: "KhachThues",
                newName: "Tenants");

            migrationBuilder.RenameTable(
                name: "HopDong",
                newName: "Contracts");

            migrationBuilder.RenameTable(
                name: "DichVu",
                newName: "Services");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "TenPhong",
                table: "Rooms",
                newName: "RoomCode");

            migrationBuilder.RenameColumn(
                name: "Tang",
                table: "Rooms",
                newName: "FloorNumber");

            migrationBuilder.RenameColumn(
                name: "SoLuongNguoi",
                table: "Rooms",
                newName: "MaxOccupants");

            migrationBuilder.RenameColumn(
                name: "NhaTroId",
                table: "Rooms",
                newName: "BuildingId");

            migrationBuilder.RenameColumn(
                name: "GiaPhong",
                table: "Rooms",
                newName: "BaseRentPrice");

            migrationBuilder.RenameColumn(
                name: "DoiTuong",
                table: "Rooms",
                newName: "RoomType");

            migrationBuilder.RenameColumn(
                name: "DienTich",
                table: "Rooms",
                newName: "Area");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Rooms",
                newName: "RoomId");

            migrationBuilder.RenameColumn(
                name: "NoiThat",
                table: "Rooms",
                newName: "Orientation");

            migrationBuilder.RenameColumn(
                name: "TenNhaTro",
                table: "Buildings",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DiaChiChiTiet",
                table: "Buildings",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "DanhSachDichVu",
                table: "Buildings",
                newName: "Utilities");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Buildings",
                newName: "BuildingId");

            migrationBuilder.RenameColumn(
                name: "SoDienThoai1",
                table: "Tenants",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "SoCCCD",
                table: "Tenants",
                newName: "CCCD");

            migrationBuilder.RenameColumn(
                name: "NgaySinh",
                table: "Tenants",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "HoTen",
                table: "Tenants",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "DiaChiThuongTru",
                table: "Tenants",
                newName: "PermanentAddress");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tenants",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "SoDienThoai2",
                table: "Tenants",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "NoiSinh",
                table: "Tenants",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "Contracts",
                newName: "ContractStatus");

            migrationBuilder.RenameColumn(
                name: "TienCoc",
                table: "Contracts",
                newName: "DepositAmount");

            migrationBuilder.RenameColumn(
                name: "PhongTroId",
                table: "Contracts",
                newName: "RoomId");

            migrationBuilder.RenameColumn(
                name: "NgayKetThuc",
                table: "Contracts",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "NgayBatDau",
                table: "Contracts",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "GiaThue",
                table: "Contracts",
                newName: "RentPriceSnapshot");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Contracts",
                newName: "ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_HopDong_PhongTroId",
                table: "Contracts",
                newName: "IX_Contracts_RoomId");

            migrationBuilder.RenameColumn(
                name: "Ten",
                table: "Services",
                newName: "ServiceName");

            migrationBuilder.RenameColumn(
                name: "LoaiDichVu",
                table: "Services",
                newName: "ChargeType");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Services",
                newName: "ServiceId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "RoomCode",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Area",
                table: "Rooms",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "Balcony",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ConditionScore",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Rooms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "HasPrivateBathroom",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastInspectionDate",
                table: "Rooms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoiseLevelRating",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Rooms",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Buildings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Buildings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ElectricityProvider",
                table: "Buildings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FireSafetyCertificateExpiry",
                table: "Buildings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Buildings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "Buildings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Buildings",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Buildings",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Buildings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OperationDate",
                table: "Buildings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalFloors",
                table: "Buildings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WaterProvider",
                table: "Buildings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CCCD",
                table: "Tenants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tenants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Tenants",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractStatus",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractCode",
                table: "Contracts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DepositStatus",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Contracts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PaymentCycle",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignatureDate",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "RoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings",
                column: "BuildingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants",
                column: "TenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "ContractId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "ServiceId");

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    AssetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarrantyMonths = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepreciationYears = table.Column<int>(type: "int", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.AssetId);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditId);
                });

            migrationBuilder.CreateTable(
                name: "ContractExtensions",
                columns: table => new
                {
                    ExtensionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    OldEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewRentPriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractExtensions", x => x.ExtensionId);
                });

            migrationBuilder.CreateTable(
                name: "ContractServices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnitPriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractTenants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    IsRepresentative = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractTenants_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "ContractId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractTenants_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonthYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                });

            migrationBuilder.CreateTable(
                name: "Liquidations",
                columns: table => new
                {
                    LiquidationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalInvoiceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepositUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liquidations", x => x.LiquidationId);
                    table.ForeignKey(
                        name: "FK_Liquidations_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "ContractId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeterReadings",
                columns: table => new
                {
                    ReadingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    MonthYear = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OldElectricityIndex = table.Column<int>(type: "int", nullable: false),
                    NewElectricityIndex = table.Column<int>(type: "int", nullable: false),
                    OldWaterIndex = table.Column<int>(type: "int", nullable: false),
                    NewWaterIndex = table.Column<int>(type: "int", nullable: false),
                    ConsumptionElectricity = table.Column<int>(type: "int", nullable: true),
                    ConsumptionWater = table.Column<int>(type: "int", nullable: true),
                    PreviousReadingId = table.Column<long>(type: "bigint", nullable: true),
                    ReadingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterReadings", x => x.ReadingId);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EvidenceImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentWebhookLogs",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentWebhookLogs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "RoomAssets",
                columns: table => new
                {
                    RoomAssetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConditionScore = table.Column<int>(type: "int", nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceCycleMonths = table.Column<int>(type: "int", nullable: true),
                    IsUnderWarranty = table.Column<bool>(type: "bit", nullable: true),
                    LocationNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAssets", x => x.RoomAssetId);
                });

            migrationBuilder.CreateTable(
                name: "RoomStatusHistories",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    NewStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationDays = table.Column<int>(type: "int", nullable: true),
                    AutoGenerated = table.Column<bool>(type: "bit", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomStatusHistories", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_RoomStatusHistories_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServicePriceHistory",
                columns: table => new
                {
                    PriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePriceHistory", x => x.PriceId);
                    table.ForeignKey(
                        name: "FK_ServicePriceHistory_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigs",
                columns: table => new
                {
                    ConfigKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConfigValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigs", x => x.ConfigKey);
                });

            migrationBuilder.CreateTable(
                name: "TenantBalances",
                columns: table => new
                {
                    BalanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BalanceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantBalances", x => x.BalanceId);
                    table.ForeignKey(
                        name: "FK_TenantBalances_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedTo = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PermissionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingId = table.Column<int>(type: "int", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: true),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingId = table.Column<int>(type: "int", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: true),
                    AssetId = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsThumbnail = table.Column<bool>(type: "bit", nullable: false),
                    UploadedBy = table.Column<int>(type: "int", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageId);
                    table.CheckConstraint("CK_Images_OnlyOneFK", "([BuildingId] IS NOT NULL AND [RoomId] IS NULL AND [AssetId] IS NULL) OR ([BuildingId] IS NULL AND [RoomId] IS NOT NULL AND [AssetId] IS NULL) OR ([BuildingId] IS NULL AND [RoomId] IS NULL AND [AssetId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Images_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Images_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "BuildingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Images_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                columns: table => new
                {
                    DetailId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnitPriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReadingId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetMaintenanceLogs",
                columns: table => new
                {
                    MaintenanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomAssetId = table.Column<int>(type: "int", nullable: false),
                    MaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssueDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetMaintenanceLogs", x => x.MaintenanceId);
                    table.ForeignKey(
                        name: "FK_AssetMaintenanceLogs_RoomAssets_RoomAssetId",
                        column: x => x.RoomAssetId,
                        principalTable: "RoomAssets",
                        principalColumn: "RoomAssetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantBalanceTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BalanceId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RelatedInvoiceId = table.Column<int>(type: "int", nullable: true),
                    RelatedPaymentId = table.Column<long>(type: "bigint", nullable: true),
                    BalanceBefore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantBalanceTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_TenantBalanceTransactions_Invoices_RelatedInvoiceId",
                        column: x => x.RelatedInvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantBalanceTransactions_PaymentTransactions_RelatedPaymentId",
                        column: x => x.RelatedPaymentId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantBalanceTransactions_TenantBalances_BalanceId",
                        column: x => x.BalanceId,
                        principalTable: "TenantBalances",
                        principalColumn: "BalanceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantBalanceTransactions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantBalanceTransactions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketImages",
                columns: table => new
                {
                    ImageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<long>(type: "bigint", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_TicketImages_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId_RoomCode",
                table: "Rooms",
                columns: new[] { "BuildingId", "RoomCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CCCD",
                table: "Tenants",
                column: "CCCD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractCode",
                table: "Contracts",
                column: "ContractCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetMaintenanceLogs_RoomAssetId",
                table: "AssetMaintenanceLogs",
                column: "RoomAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractTenants_ContractId",
                table: "ContractTenants",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractTenants_TenantId",
                table: "ContractTenants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_AssetId",
                table: "Images",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_BuildingId",
                table: "Images",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_RoomId",
                table: "Images",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceId",
                table: "InvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceCode",
                table: "Invoices",
                column: "InvoiceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Liquidations_ContractId",
                table: "Liquidations",
                column: "ContractId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_RoomId_MonthYear",
                table: "MeterReadings",
                columns: new[] { "RoomId", "MonthYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_TransactionCode",
                table: "PaymentTransactions",
                column: "TransactionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomStatusHistories_RoomId",
                table: "RoomStatusHistories",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePriceHistory_ServiceId",
                table: "ServicePriceHistory",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantBalances_TenantId",
                table: "TenantBalances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantBalanceTransactions_BalanceId",
                table: "TenantBalanceTransactions",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantBalanceTransactions_CreatedBy",
                table: "TenantBalanceTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TenantBalanceTransactions_RelatedInvoiceId",
                table: "TenantBalanceTransactions",
                column: "RelatedInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantBalanceTransactions_RelatedPaymentId",
                table: "TenantBalanceTransactions",
                column: "RelatedPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantBalanceTransactions_TenantId",
                table: "TenantBalanceTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketImages_TicketId",
                table: "TicketImages",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Rooms_RoomId",
                table: "Contracts",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "BuildingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Rooms_RoomId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AssetMaintenanceLogs");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ContractExtensions");

            migrationBuilder.DropTable(
                name: "ContractServices");

            migrationBuilder.DropTable(
                name: "ContractTenants");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "Liquidations");

            migrationBuilder.DropTable(
                name: "MeterReadings");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PaymentWebhookLogs");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "RoomStatusHistories");

            migrationBuilder.DropTable(
                name: "ServicePriceHistory");

            migrationBuilder.DropTable(
                name: "SystemConfigs");

            migrationBuilder.DropTable(
                name: "TenantBalanceTransactions");

            migrationBuilder.DropTable(
                name: "TicketImages");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "RoomAssets");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "TenantBalances");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_CCCD",
                table: "Tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_BuildingId_RoomCode",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractCode",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Balcony",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ConditionScore",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "HasPrivateBathroom",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LastInspectionDate",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "NoiseLevelRating",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ContractCode",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DepositStatus",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PaymentCycle",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SignatureDate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "ElectricityProvider",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "FireSafetyCertificateExpiry",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "LastMaintenanceDate",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "OperationDate",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "TotalFloors",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "WaterProvider",
                table: "Buildings");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "KhachThues");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "DichVu");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "PhongTros");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "HopDong");

            migrationBuilder.RenameTable(
                name: "Buildings",
                newName: "NhaTros");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "KhachThues",
                newName: "SoDienThoai1");

            migrationBuilder.RenameColumn(
                name: "PermanentAddress",
                table: "KhachThues",
                newName: "DiaChiThuongTru");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "KhachThues",
                newName: "HoTen");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "KhachThues",
                newName: "NgaySinh");

            migrationBuilder.RenameColumn(
                name: "CCCD",
                table: "KhachThues",
                newName: "SoCCCD");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "KhachThues",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "KhachThues",
                newName: "NoiSinh");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "KhachThues",
                newName: "SoDienThoai2");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "DichVu",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "ChargeType",
                table: "DichVu",
                newName: "LoaiDichVu");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "DichVu",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RoomType",
                table: "PhongTros",
                newName: "DoiTuong");

            migrationBuilder.RenameColumn(
                name: "RoomCode",
                table: "PhongTros",
                newName: "TenPhong");

            migrationBuilder.RenameColumn(
                name: "MaxOccupants",
                table: "PhongTros",
                newName: "SoLuongNguoi");

            migrationBuilder.RenameColumn(
                name: "FloorNumber",
                table: "PhongTros",
                newName: "Tang");

            migrationBuilder.RenameColumn(
                name: "BuildingId",
                table: "PhongTros",
                newName: "NhaTroId");

            migrationBuilder.RenameColumn(
                name: "BaseRentPrice",
                table: "PhongTros",
                newName: "GiaPhong");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "PhongTros",
                newName: "DienTich");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "PhongTros",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Orientation",
                table: "PhongTros",
                newName: "NoiThat");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "HopDong",
                newName: "NgayBatDau");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "HopDong",
                newName: "PhongTroId");

            migrationBuilder.RenameColumn(
                name: "RentPriceSnapshot",
                table: "HopDong",
                newName: "GiaThue");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "HopDong",
                newName: "NgayKetThuc");

            migrationBuilder.RenameColumn(
                name: "DepositAmount",
                table: "HopDong",
                newName: "TienCoc");

            migrationBuilder.RenameColumn(
                name: "ContractStatus",
                table: "HopDong",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "ContractId",
                table: "HopDong",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_RoomId",
                table: "HopDong",
                newName: "IX_HopDong_PhongTroId");

            migrationBuilder.RenameColumn(
                name: "Utilities",
                table: "NhaTros",
                newName: "DanhSachDichVu");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "NhaTros",
                newName: "TenNhaTro");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "NhaTros",
                newName: "DiaChiChiTiet");

            migrationBuilder.RenameColumn(
                name: "BuildingId",
                table: "NhaTros",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoCCCD",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoiCap",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienCoc",
                table: "KhachThues",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DonGia",
                table: "DichVu",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "DichVu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TenPhong",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<double>(
                name: "DienTich",
                table: "PhongTros",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<string>(
                name: "AnhPhong",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DichVu",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TienCoc",
                table: "PhongTros",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayKetThuc",
                table: "HopDong",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "TrangThai",
                table: "HopDong",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "GhiChuThanhLy",
                table: "HopDong",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayThanhLy",
                table: "HopDong",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienHoanTra",
                table: "HopDong",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TongTienQuyetToan",
                table: "HopDong",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "NhaTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "GiaThue",
                table: "NhaTros",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LoaiNha",
                table: "NhaTros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NgayThuTien",
                table: "NhaTros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PhuongXa",
                table: "NhaTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuanHuyen",
                table: "NhaTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TinhThanh",
                table: "NhaTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhachThues",
                table: "KhachThues",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DichVu",
                table: "DichVu",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhongTros",
                table: "PhongTros",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HopDong",
                table: "HopDong",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhaTros",
                table: "NhaTros",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTros_NhaTroId",
                table: "PhongTros",
                column: "NhaTroId");

            migrationBuilder.AddForeignKey(
                name: "FK_HopDong_PhongTros_PhongTroId",
                table: "HopDong",
                column: "PhongTroId",
                principalTable: "PhongTros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhongTros_NhaTros_NhaTroId",
                table: "PhongTros",
                column: "NhaTroId",
                principalTable: "NhaTros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
