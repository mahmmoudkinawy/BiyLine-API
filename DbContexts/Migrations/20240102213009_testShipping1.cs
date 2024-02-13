using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class testShipping1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_CommercialRegisterImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_IDImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_ProfileImageId",
                table: "ShippingCompanies");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_CommercialRegisterImageId",
                table: "ShippingCompanies",
                column: "CommercialRegisterImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_IDImageId",
                table: "ShippingCompanies",
                column: "IDImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_ProfileImageId",
                table: "ShippingCompanies",
                column: "ProfileImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_CommercialRegisterImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_IDImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_ProfileImageId",
                table: "ShippingCompanies");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_CommercialRegisterImageId",
                table: "ShippingCompanies",
                column: "CommercialRegisterImageId",
                unique: true,
                filter: "[CommercialRegisterImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_IDImageId",
                table: "ShippingCompanies",
                column: "IDImageId",
                unique: true,
                filter: "[IDImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_ProfileImageId",
                table: "ShippingCompanies",
                column: "ProfileImageId",
                unique: true,
                filter: "[ProfileImageId] IS NOT NULL");
        }
    }
}
