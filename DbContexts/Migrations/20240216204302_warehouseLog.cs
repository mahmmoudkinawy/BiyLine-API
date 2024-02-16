using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class warehouseLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductVariationEntity",
                table: "ShipmentDetails");

            migrationBuilder.AddColumn<int>(
                name: "CashOutType",
                table: "Shipments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WarehouseLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ProductVariationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseLogs_ProductVariations_ProductVariationId",
                        column: x => x.ProductVariationId,
                        principalTable: "ProductVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_WarehouseLogs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_WarehouseLogs_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLogs_ProductId",
                table: "WarehouseLogs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLogs_ProductVariationId",
                table: "WarehouseLogs",
                column: "ProductVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLogs_WarehouseId",
                table: "WarehouseLogs",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseLogs");

            migrationBuilder.DropColumn(
                name: "CashOutType",
                table: "Shipments");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariationEntity",
                table: "ShipmentDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
