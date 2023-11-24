using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCouponProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouponProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouponEntity",
                table: "CouponEntity");

            migrationBuilder.RenameTable(
                name: "CouponEntity",
                newName: "Coupons");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Coupons",
                table: "Coupons",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Coupons",
                table: "Coupons");

            migrationBuilder.RenameTable(
                name: "Coupons",
                newName: "CouponEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouponEntity",
                table: "CouponEntity",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CouponProducts",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CouponId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponProducts", x => new { x.ProductId, x.CouponId });
                    table.ForeignKey(
                        name: "FK_CouponProducts_CouponEntity_CouponId",
                        column: x => x.CouponId,
                        principalTable: "CouponEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouponProducts_CouponId",
                table: "CouponProducts",
                column: "CouponId");
        }
    }
}
