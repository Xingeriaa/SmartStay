using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace do_an_tot_nghiep.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomAnalyticsSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomAnalyticsSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingId = table.Column<int>(type: "int", nullable: false),
                    SnapshotYear = table.Column<int>(type: "int", nullable: false),
                    SnapshotMonth = table.Column<int>(type: "int", nullable: false),
                    TotalRooms = table.Column<int>(type: "int", nullable: false),
                    OccupiedRooms = table.Column<int>(type: "int", nullable: false),
                    VacantRooms = table.Column<int>(type: "int", nullable: false),
                    MaintenanceRooms = table.Column<int>(type: "int", nullable: false),
                    OccupancyRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TotalMaintenanceCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpectedRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAnalyticsSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomAnalyticsSnapshots_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "BuildingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomAnalyticsSnapshots_BuildingId",
                table: "RoomAnalyticsSnapshots",
                column: "BuildingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomAnalyticsSnapshots");
        }
    }
}
