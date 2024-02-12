using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreToBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Baskets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_StoreId",
                table: "Baskets",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Stores_StoreId",
                table: "Baskets",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Stores_StoreId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_StoreId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Baskets");
        }
    }
}
