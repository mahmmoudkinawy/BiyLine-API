using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class addShipmentsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanyGovernorateDetails_Governments_GovernorateEntityId",
                table: "ShippingCompanyGovernorateDetails");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanyGovernorateDetails_GovernorateEntityId",
                table: "ShippingCompanyGovernorateDetails");

            migrationBuilder.DropColumn(
                name: "GovernorateEntityId",
                table: "ShippingCompanyGovernorateDetails");

            migrationBuilder.AddColumn<int>(
                name: "GovernorateId",
                table: "ShippingCompanyGovernorateDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "CommissionRate",
            //    table: "Coupons",
            //    type: "decimal(18,2)",
            //    nullable: true);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "DiscountPercentage",
            //    table: "Coupons",
            //    type: "decimal(18,2)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Name",
            //    table: "Coupons",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "CouponCategory",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryId = table.Column<int>(type: "int", nullable: false),
            //        CouponId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CouponCategory", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_CouponCategory_Categories_CategoryId",
            //            column: x => x.CategoryId,
            //            principalTable: "Categories",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_CouponCategory_Coupons_CouponId",
            //            column: x => x.CouponId,
            //            principalTable: "Coupons",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CouponsUsages",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CouponId = table.Column<int>(type: "int", nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        ItemCount = table.Column<int>(type: "int", nullable: true),
            //        UserId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CouponsUsages", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_CouponsUsages_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_CouponsUsages_Coupons_CouponId",
            //            column: x => x.CouponId,
            //            principalTable: "Coupons",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "PickUpPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GovernorateId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickUpPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickUpPoints_Governments_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "ShippingCompanyGovernorates",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ShippingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        ShippingCompanyId = table.Column<int>(type: "int", nullable: false),
            //        GovernorateId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ShippingCompanyGovernorates", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ShippingCompanyGovernorates_Governments_GovernorateId",
            //            column: x => x.GovernorateId,
            //            principalTable: "Governments",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ShippingCompanyGovernorates_ShippingCompanies_ShippingCompanyId",
            //            column: x => x.ShippingCompanyId,
            //            principalTable: "ShippingCompanies",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "StoreWalletLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreWalletId = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LogStatus = table.Column<int>(type: "int", nullable: true),
                    EmpId = table.Column<int>(type: "int", nullable: true),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    DocumentType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreWalletLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreWalletLogs_AspNetUsers_EmpId",
                        column: x => x.EmpId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoreWalletLogs_StoreWallets_StoreWalletId",
                        column: x => x.StoreWalletId,
                        principalTable: "StoreWallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalDiscountPercentage = table.Column<double>(type: "float", nullable: true),
                    ValueAddedTax = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CollectingShipmentCost = table.Column<int>(type: "int", nullable: false),
                    CollectingDeliveryCost = table.Column<int>(type: "int", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: false),
                    DetailedAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingCost = table.Column<double>(type: "float", nullable: false),
                    ShippingCompanyId = table.Column<int>(type: "int", nullable: true),
                    PickUpPointId = table.Column<int>(type: "int", nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: true),
                    StoreWalletId = table.Column<int>(type: "int", nullable: true),
                    PaidAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Governments_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shipments_PickUpPoints_PickUpPointId",
                        column: x => x.PickUpPointId,
                        principalTable: "PickUpPoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_ShippingCompanies_ShippingCompanyId",
                        column: x => x.ShippingCompanyId,
                        principalTable: "ShippingCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_StoreWallets_StoreWalletId",
                        column: x => x.StoreWalletId,
                        principalTable: "StoreWallets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductVariationEntity = table.Column<int>(type: "int", nullable: false),
                    ProductVariationId = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<double>(type: "float", nullable: false),
                    DiscountPercentage = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentDetails_ProductVariations_ProductVariationId",
                        column: x => x.ProductVariationId,
                        principalTable: "ProductVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentDetails_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorateDetails_GovernorateId",
                table: "ShippingCompanyGovernorateDetails",
                column: "GovernorateId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CouponCategory_CategoryId",
            //    table: "CouponCategory",
            //    column: "CategoryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CouponCategory_CouponId",
            //    table: "CouponCategory",
            //    column: "CouponId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CouponsUsages_CouponId",
            //    table: "CouponsUsages",
            //    column: "CouponId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CouponsUsages_UserId",
            //    table: "CouponsUsages",
            //    column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PickUpPoints_GovernorateId",
                table: "PickUpPoints",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_ProductId",
                table: "ShipmentDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_ProductVariationId",
                table: "ShipmentDetails",
                column: "ProductVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_ShipmentId",
                table: "ShipmentDetails",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_GovernorateId",
                table: "Shipments",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_PickUpPointId",
                table: "Shipments",
                column: "PickUpPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShippingCompanyId",
                table: "Shipments",
                column: "ShippingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_StoreWalletId",
                table: "Shipments",
                column: "StoreWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_WarehouseId",
                table: "Shipments",
                column: "WarehouseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ShippingCompanyGovernorates_GovernorateId",
            //    table: "ShippingCompanyGovernorates",
            //    column: "GovernorateId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ShippingCompanyGovernorates_ShippingCompanyId",
            //    table: "ShippingCompanyGovernorates",
            //    column: "ShippingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreWalletLogs_EmpId",
                table: "StoreWalletLogs",
                column: "EmpId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreWalletLogs_StoreWalletId",
                table: "StoreWalletLogs",
                column: "StoreWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanyGovernorateDetails_Governments_GovernorateId",
                table: "ShippingCompanyGovernorateDetails",
                column: "GovernorateId",
                principalTable: "Governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanyGovernorateDetails_Governments_GovernorateId",
                table: "ShippingCompanyGovernorateDetails");

            //migrationBuilder.DropTable(
            //    name: "CouponCategory");

            //migrationBuilder.DropTable(
            //    name: "CouponsUsages");

            migrationBuilder.DropTable(
                name: "ShipmentDetails");

            //migrationBuilder.DropTable(
            //    name: "ShippingCompanyGovernorates");

            migrationBuilder.DropTable(
                name: "StoreWalletLogs");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "PickUpPoints");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanyGovernorateDetails_GovernorateId",
                table: "ShippingCompanyGovernorateDetails");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "ShippingCompanyGovernorateDetails");

            //migrationBuilder.DropColumn(
            //    name: "CommissionRate",
            //    table: "Coupons");

            //migrationBuilder.DropColumn(
            //    name: "DiscountPercentage",
            //    table: "Coupons");

            //migrationBuilder.DropColumn(
            //    name: "Name",
            //    table: "Coupons");

            migrationBuilder.AddColumn<int>(
                name: "GovernorateEntityId",
                table: "ShippingCompanyGovernorateDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorateDetails_GovernorateEntityId",
                table: "ShippingCompanyGovernorateDetails",
                column: "GovernorateEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanyGovernorateDetails_Governments_GovernorateEntityId",
                table: "ShippingCompanyGovernorateDetails",
                column: "GovernorateEntityId",
                principalTable: "Governments",
                principalColumn: "Id");
        }
    }
}
