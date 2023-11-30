using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class CreateContractOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractOrderProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractOrderProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractOrderProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromStoreId = table.Column<int>(type: "int", nullable: false),
                    ToStoreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractOrders_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOrders_Stores_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractOrderVariations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductVariationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractOrderVariations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractOrderVariations_ProductVariations_ProductVariationId",
                        column: x => x.ProductVariationId,
                        principalTable: "ProductVariations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractOrderEntityContractOrderProductEntity",
                columns: table => new
                {
                    ContractOrderProductsId = table.Column<int>(type: "int", nullable: false),
                    ContractOrdersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractOrderEntityContractOrderProductEntity", x => new { x.ContractOrderProductsId, x.ContractOrdersId });
                    table.ForeignKey(
                        name: "FK_ContractOrderEntityContractOrderProductEntity_ContractOrderProducts_ContractOrderProductsId",
                        column: x => x.ContractOrderProductsId,
                        principalTable: "ContractOrderProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractOrderEntityContractOrderProductEntity_ContractOrders_ContractOrdersId",
                        column: x => x.ContractOrdersId,
                        principalTable: "ContractOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractOrderProductEntityContractOrderVariationEntity",
                columns: table => new
                {
                    ContractOrderProductsId = table.Column<int>(type: "int", nullable: false),
                    ContractOrderVariationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractOrderProductEntityContractOrderVariationEntity", x => new { x.ContractOrderProductsId, x.ContractOrderVariationsId });
                    table.ForeignKey(
                        name: "FK_ContractOrderProductEntityContractOrderVariationEntity_ContractOrderProducts_ContractOrderProductsId",
                        column: x => x.ContractOrderProductsId,
                        principalTable: "ContractOrderProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractOrderProductEntityContractOrderVariationEntity_ContractOrderVariations_ContractOrderVariationsId",
                        column: x => x.ContractOrderVariationsId,
                        principalTable: "ContractOrderVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractOrderEntityContractOrderProductEntity_ContractOrdersId",
                table: "ContractOrderEntityContractOrderProductEntity",
                column: "ContractOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOrderProductEntityContractOrderVariationEntity_ContractOrderVariationsId",
                table: "ContractOrderProductEntityContractOrderVariationEntity",
                column: "ContractOrderVariationsId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOrderProducts_ProductId",
                table: "ContractOrderProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOrders_FromStoreId",
                table: "ContractOrders",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOrders_ToStoreId",
                table: "ContractOrders",
                column: "ToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOrderVariations_ProductVariationId",
                table: "ContractOrderVariations",
                column: "ProductVariationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractOrderEntityContractOrderProductEntity");

            migrationBuilder.DropTable(
                name: "ContractOrderProductEntityContractOrderVariationEntity");

            migrationBuilder.DropTable(
                name: "ContractOrders");

            migrationBuilder.DropTable(
                name: "ContractOrderProducts");

            migrationBuilder.DropTable(
                name: "ContractOrderVariations");
        }
    }
}
