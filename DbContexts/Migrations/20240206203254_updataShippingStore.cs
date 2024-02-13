using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class updataShippingStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Stores_StoreId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_StoreId",
                table: "ShippingCompanies");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "ShippingCompanies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_StoreId",
                table: "ShippingCompanies",
                column: "StoreId",
                unique: true,
                filter: "[StoreId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Stores_StoreId",
                table: "ShippingCompanies",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Stores_StoreId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_StoreId",
                table: "ShippingCompanies");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "ShippingCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_StoreId",
                table: "ShippingCompanies",
                column: "StoreId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Stores_StoreId",
                table: "ShippingCompanies",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
