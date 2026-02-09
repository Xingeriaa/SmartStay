using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace do_an_tot_nghiep.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKhachThueFullFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SoDienThoai",
                table: "KhachThues",
                newName: "SoDienThoai2");

            migrationBuilder.RenameColumn(
                name: "QueQuan",
                table: "KhachThues",
                newName: "SoDienThoai1");

            migrationBuilder.AddColumn<string>(
                name: "DiaChiThuongTru",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NoiCap",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NoiSinh",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienCoc",
                table: "KhachThues",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaChiThuongTru",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "NoiCap",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "NoiSinh",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "SoTienCoc",
                table: "KhachThues");

            migrationBuilder.RenameColumn(
                name: "SoDienThoai2",
                table: "KhachThues",
                newName: "SoDienThoai");

            migrationBuilder.RenameColumn(
                name: "SoDienThoai1",
                table: "KhachThues",
                newName: "QueQuan");
        }
    }
}
