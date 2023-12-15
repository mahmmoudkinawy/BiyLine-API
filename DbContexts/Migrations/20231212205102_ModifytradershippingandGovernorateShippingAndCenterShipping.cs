using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class ModifytradershippingandGovernorateShippingAndCenterShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "TraderShippingCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TraderShippingCompanies_StoreId",
                table: "TraderShippingCompanies",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_TraderShippingCompanies_Stores_StoreId",
                table: "TraderShippingCompanies",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraderShippingCompanies_Stores_StoreId",
                table: "TraderShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_TraderShippingCompanies_StoreId",
                table: "TraderShippingCompanies");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "TraderShippingCompanies");
        }
    }
}
