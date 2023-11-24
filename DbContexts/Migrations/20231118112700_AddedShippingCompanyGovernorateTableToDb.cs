using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedShippingCompanyGovernorateTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Governments_GovernorateId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_GovernorateId",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "ShippingCompanies");

            migrationBuilder.CreateTable(
                name: "ShippingCompanyGovernorates",
                columns: table => new
                {
                    ShippingCompanyId = table.Column<int>(type: "int", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_ShippingCompanyGovernorates_GovernorateId",
                table: "ShippingCompanyGovernorates",
                column: "GovernorateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingCompanyGovernorates");

            migrationBuilder.AddColumn<int>(
                name: "GovernorateId",
                table: "ShippingCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_GovernorateId",
                table: "ShippingCompanies",
                column: "GovernorateId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Governments_GovernorateId",
                table: "ShippingCompanies",
                column: "GovernorateId",
                principalTable: "Governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
