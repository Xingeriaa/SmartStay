using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace do_an_tot_nghiep.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UserPermissions",
                newName: "UserPermissions",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Tickets",
                newName: "Tickets",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "TicketImages",
                newName: "TicketImages",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "Tenants",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "TenantBalanceTransactions",
                newName: "TenantBalanceTransactions",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "TenantBalances",
                newName: "TenantBalances",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SystemConfigs",
                newName: "SystemConfigs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Services",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ServicePriceHistory",
                newName: "ServicePriceHistory",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "RoomStatusHistories",
                newName: "RoomStatusHistories",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Rooms",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "RoomAssets",
                newName: "RoomAssets",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "RoomAnalyticsSnapshots",
                newName: "RoomAnalyticsSnapshots",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PaymentWebhookLogs",
                newName: "PaymentWebhookLogs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PaymentTransactions",
                newName: "PaymentTransactions",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notifications",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "MeterReadings",
                newName: "MeterReadings",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Liquidations",
                newName: "Liquidations",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoices",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "InvoiceDetails",
                newName: "InvoiceDetails",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "Images",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ContractTenants",
                newName: "ContractTenants",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ContractServices",
                newName: "ContractServices",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "Contracts",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ContractExtensions",
                newName: "ContractExtensions",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Buildings",
                newName: "Buildings",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLogs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Assets",
                newName: "Assets",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AssetMaintenanceLogs",
                newName: "AssetMaintenanceLogs",
                newSchema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "dbo",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "UserPermissions",
                schema: "dbo",
                newName: "UserPermissions");

            migrationBuilder.RenameTable(
                name: "Tickets",
                schema: "dbo",
                newName: "Tickets");

            migrationBuilder.RenameTable(
                name: "TicketImages",
                schema: "dbo",
                newName: "TicketImages");

            migrationBuilder.RenameTable(
                name: "Tenants",
                schema: "dbo",
                newName: "Tenants");

            migrationBuilder.RenameTable(
                name: "TenantBalanceTransactions",
                schema: "dbo",
                newName: "TenantBalanceTransactions");

            migrationBuilder.RenameTable(
                name: "TenantBalances",
                schema: "dbo",
                newName: "TenantBalances");

            migrationBuilder.RenameTable(
                name: "SystemConfigs",
                schema: "dbo",
                newName: "SystemConfigs");

            migrationBuilder.RenameTable(
                name: "Services",
                schema: "dbo",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "ServicePriceHistory",
                schema: "dbo",
                newName: "ServicePriceHistory");

            migrationBuilder.RenameTable(
                name: "RoomStatusHistories",
                schema: "dbo",
                newName: "RoomStatusHistories");

            migrationBuilder.RenameTable(
                name: "Rooms",
                schema: "dbo",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "RoomAssets",
                schema: "dbo",
                newName: "RoomAssets");

            migrationBuilder.RenameTable(
                name: "RoomAnalyticsSnapshots",
                schema: "dbo",
                newName: "RoomAnalyticsSnapshots");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "dbo",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "PaymentWebhookLogs",
                schema: "dbo",
                newName: "PaymentWebhookLogs");

            migrationBuilder.RenameTable(
                name: "PaymentTransactions",
                schema: "dbo",
                newName: "PaymentTransactions");

            migrationBuilder.RenameTable(
                name: "Notifications",
                schema: "dbo",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "MeterReadings",
                schema: "dbo",
                newName: "MeterReadings");

            migrationBuilder.RenameTable(
                name: "Liquidations",
                schema: "dbo",
                newName: "Liquidations");

            migrationBuilder.RenameTable(
                name: "Invoices",
                schema: "dbo",
                newName: "Invoices");

            migrationBuilder.RenameTable(
                name: "InvoiceDetails",
                schema: "dbo",
                newName: "InvoiceDetails");

            migrationBuilder.RenameTable(
                name: "Images",
                schema: "dbo",
                newName: "Images");

            migrationBuilder.RenameTable(
                name: "ContractTenants",
                schema: "dbo",
                newName: "ContractTenants");

            migrationBuilder.RenameTable(
                name: "ContractServices",
                schema: "dbo",
                newName: "ContractServices");

            migrationBuilder.RenameTable(
                name: "Contracts",
                schema: "dbo",
                newName: "Contracts");

            migrationBuilder.RenameTable(
                name: "ContractExtensions",
                schema: "dbo",
                newName: "ContractExtensions");

            migrationBuilder.RenameTable(
                name: "Buildings",
                schema: "dbo",
                newName: "Buildings");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                schema: "dbo",
                newName: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "Assets",
                schema: "dbo",
                newName: "Assets");

            migrationBuilder.RenameTable(
                name: "AssetMaintenanceLogs",
                schema: "dbo",
                newName: "AssetMaintenanceLogs");
        }
    }
}
