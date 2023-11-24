using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedRelationshipBetweenStoresAndCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Coupons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_StoreId",
                table: "Coupons",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Stores_StoreId",
                table: "Coupons",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Stores_StoreId",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_StoreId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Coupons");
        }
    }
}
