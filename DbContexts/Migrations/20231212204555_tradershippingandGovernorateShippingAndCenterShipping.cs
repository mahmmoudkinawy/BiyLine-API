using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class tradershippingandGovernorateShippingAndCenterShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TraderShippingCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraderShippingCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GovernorateShippings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PickupPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeightTo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerExtraKilo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: false),
                    TraderShippingCompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GovernorateShippings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GovernorateShippings_Governments_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GovernorateShippings_TraderShippingCompanies_TraderShippingCompanyId",
                        column: x => x.TraderShippingCompanyId,
                        principalTable: "TraderShippingCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CenterShippings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PickupPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeightTo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerExtraKilo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GovernorateShippingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterShippings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterShippings_GovernorateShippings_GovernorateShippingId",
                        column: x => x.GovernorateShippingId,
                        principalTable: "GovernorateShippings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CenterShippings_GovernorateShippingId",
                table: "CenterShippings",
                column: "GovernorateShippingId");

            migrationBuilder.CreateIndex(
                name: "IX_GovernorateShippings_GovernorateId",
                table: "GovernorateShippings",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_GovernorateShippings_TraderShippingCompanyId",
                table: "GovernorateShippings",
                column: "TraderShippingCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CenterShippings");

            migrationBuilder.DropTable(
                name: "GovernorateShippings");

            migrationBuilder.DropTable(
                name: "TraderShippingCompanies");
        }
    }
}
