using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CenterShippings");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Baskets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_AddressId",
                table: "Baskets",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Addresses_AddressId",
                table: "Baskets",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Addresses_AddressId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_AddressId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Baskets");

            migrationBuilder.CreateTable(
                name: "CenterShippings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GovernorateShippingId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerExtraKilo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeightTo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
        }
    }
}
