using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreToShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Shipments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_StoreId",
                table: "Shipments",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Stores_StoreId",
                table: "Shipments",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Stores_StoreId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_StoreId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Shipments");
        }
    }
}
