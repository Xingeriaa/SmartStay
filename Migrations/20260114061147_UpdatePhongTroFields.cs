using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace do_an_tot_nghiep.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhongTroFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<double>(
                name: "DienTich",
                table: "PhongTros",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "DoiTuong",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoiThat",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoLuongNguoi",
                table: "PhongTros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Tang",
                table: "PhongTros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TienCoc",
                table: "PhongTros",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnhPhong",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "DichVu",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "DienTich",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "DoiTuong",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "NoiThat",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "SoLuongNguoi",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "Tang",
                table: "PhongTros");

            migrationBuilder.DropColumn(
                name: "TienCoc",
                table: "PhongTros");
        }
    }
}
