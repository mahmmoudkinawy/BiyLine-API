using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class CreateSupplierInvoiceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierInvoiceId",
                table: "ContractOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SupplierInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Returned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierInvoices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractOrders_SupplierInvoiceId",
                table: "ContractOrders",
                column: "SupplierInvoiceId",
                unique: true,
                filter: "[SupplierInvoiceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractOrders_SupplierInvoices_SupplierInvoiceId",
                table: "ContractOrders",
                column: "SupplierInvoiceId",
                principalTable: "SupplierInvoices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractOrders_SupplierInvoices_SupplierInvoiceId",
                table: "ContractOrders");

            migrationBuilder.DropTable(
                name: "SupplierInvoices");

            migrationBuilder.DropIndex(
                name: "IX_ContractOrders_SupplierInvoiceId",
                table: "ContractOrders");

            migrationBuilder.DropColumn(
                name: "SupplierInvoiceId",
                table: "ContractOrders");
        }
    }
}
