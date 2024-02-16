using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class fixSnapshotFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Stores_StoreId",
                table: "ShippingCompanies");

            migrationBuilder.DropTable(
                name: "ShippingCompanyGovernorates");

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

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ShippingCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Collection",
                table: "ShippingCompanies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryCases",
                table: "ShippingCompanies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "ShippingCompanies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserEntityId",
                table: "ShippingCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippingCompanyEntityID",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionRate",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CouponCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CouponId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponCategory_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CouponsUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemCount = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponsUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponsUsages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponsUsages_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingCompanyGovernorateDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PickUpCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    OverweightFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: false),
                    ShippingCompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCompanyGovernorateDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingCompanyGovernorateDetails_Governments_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingCompanyGovernorateDetails_ShippingCompanies_ShippingCompanyId",
                        column: x => x.ShippingCompanyId,
                        principalTable: "ShippingCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_StoreId",
                table: "ShippingCompanies",
                column: "StoreId",
                unique: true,
                filter: "[StoreId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_UserEntityId",
                table: "ShippingCompanies",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ShippingCompanyEntityID",
                table: "Images",
                column: "ShippingCompanyEntityID");

            migrationBuilder.CreateIndex(
                name: "IX_CouponCategory_CategoryId",
                table: "CouponCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponCategory_CouponId",
                table: "CouponCategory",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponsUsages_CouponId",
                table: "CouponsUsages",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponsUsages_UserId",
                table: "CouponsUsages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorateDetails_GovernorateId",
                table: "ShippingCompanyGovernorateDetails",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorateDetails_ShippingCompanyId",
                table: "ShippingCompanyGovernorateDetails",
                column: "ShippingCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ShippingCompanies_ShippingCompanyEntityID",
                table: "Images",
                column: "ShippingCompanyEntityID",
                principalTable: "ShippingCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_AspNetUsers_UserEntityId",
                table: "ShippingCompanies",
                column: "UserEntityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Images_ShippingCompanies_ShippingCompanyEntityID",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_AspNetUsers_UserEntityId",
                table: "ShippingCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Stores_StoreId",
                table: "ShippingCompanies");

            migrationBuilder.DropTable(
                name: "CouponCategory");

            migrationBuilder.DropTable(
                name: "CouponsUsages");

            migrationBuilder.DropTable(
                name: "ShippingCompanyGovernorateDetails");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_StoreId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_UserEntityId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_Images_ShippingCompanyEntityID",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "Collection",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "DeliveryCases",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "ShippingCompanyEntityID",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "CommissionRate",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Coupons");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "ShippingCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ShippingCompanyGovernorates",
                columns: table => new
                {
                    ShippingCompanyId = table.Column<int>(type: "int", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: false),
                    ShippingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCompanyGovernorates", x => new { x.ShippingCompanyId, x.GovernorateId });
                    table.ForeignKey(
                        name: "FK_ShippingCompanyGovernorates_Governments_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingCompanyGovernorates_ShippingCompanies_ShippingCompanyId",
                        column: x => x.ShippingCompanyId,
                        principalTable: "ShippingCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_StoreId",
                table: "ShippingCompanies",
                column: "StoreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorates_GovernorateId",
                table: "ShippingCompanyGovernorates",
                column: "GovernorateId");

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
