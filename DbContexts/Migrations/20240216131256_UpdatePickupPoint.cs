using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePickupPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "PickUpPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PickUpPoints_StoreId",
                table: "PickUpPoints",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickUpPoints_Stores_StoreId",
                table: "PickUpPoints",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickUpPoints_Stores_StoreId",
                table: "PickUpPoints");

            migrationBuilder.DropIndex(
                name: "IX_PickUpPoints_StoreId",
                table: "PickUpPoints");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PickUpPoints");
        }
    }
}
