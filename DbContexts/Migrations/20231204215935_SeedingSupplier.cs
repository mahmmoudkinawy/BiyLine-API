using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class SeedingSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AmountPaid",
                table: "SupplierInvoices",
                newName: "RemainingAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "SupplierInvoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "SupplierInvoices");

            migrationBuilder.RenameColumn(
                name: "RemainingAmount",
                table: "SupplierInvoices",
                newName: "AmountPaid");
        }
    }
}
