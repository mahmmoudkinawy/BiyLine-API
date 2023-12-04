using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class MakeRelationshipBetweenContractOrderProductEntityWithQuantityPricingTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityPricingTierId",
                table: "ContractOrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContractOrderProducts_QuantityPricingTierId",
                table: "ContractOrderProducts",
                column: "QuantityPricingTierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractOrderProducts_QuantityPricingTiers_QuantityPricingTierId",
                table: "ContractOrderProducts",
                column: "QuantityPricingTierId",
                principalTable: "QuantityPricingTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractOrderProducts_QuantityPricingTiers_QuantityPricingTierId",
                table: "ContractOrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_ContractOrderProducts_QuantityPricingTierId",
                table: "ContractOrderProducts");

            migrationBuilder.DropColumn(
                name: "QuantityPricingTierId",
                table: "ContractOrderProducts");
        }
    }
}
