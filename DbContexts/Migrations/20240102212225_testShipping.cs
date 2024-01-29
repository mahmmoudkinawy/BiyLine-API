using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class testShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommercialRegisterImageId",
                table: "ShippingCompanies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDImageId",
                table: "ShippingCompanies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfileImageId",
                table: "ShippingCompanies",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Images_CommercialRegisterImageId",
                table: "ShippingCompanies",
                column: "CommercialRegisterImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Images_IDImageId",
                table: "ShippingCompanies",
                column: "IDImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Images_ProfileImageId",
                table: "ShippingCompanies",
                column: "ProfileImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Images_CommercialRegisterImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Images_IDImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Images_ProfileImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_CommercialRegisterImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_IDImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_ProfileImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "CommercialRegisterImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "IDImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "ProfileImageId",
                table: "ShippingCompanies");
        }
    }
}
