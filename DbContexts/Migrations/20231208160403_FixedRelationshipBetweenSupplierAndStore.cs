using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class FixedRelationshipBetweenSupplierAndStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractOrderProducts_QuantityPricingTiers_QuantityPricingTierId",
                table: "ContractOrderProducts");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "ContractOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuantityPricingTierId",
                table: "ContractOrderProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractOrderProducts_QuantityPricingTiers_QuantityPricingTierId",
                table: "ContractOrderProducts",
                column: "QuantityPricingTierId",
                principalTable: "QuantityPricingTiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractOrderProducts_QuantityPricingTiers_QuantityPricingTierId",
                table: "ContractOrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "ContractOrders");

            migrationBuilder.AlterColumn<int>(
                name: "QuantityPricingTierId",
                table: "ContractOrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractOrderProducts_QuantityPricingTiers_QuantityPricingTierId",
                table: "ContractOrderProducts",
                column: "QuantityPricingTierId",
                principalTable: "QuantityPricingTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
