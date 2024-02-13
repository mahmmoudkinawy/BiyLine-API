using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class updateShippingCompany : Migration
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

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorateDetails_GovernorateId",
                table: "ShippingCompanyGovernorateDetails",
                column: "GovernorateId");

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

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanyGovernorateDetails_GovernorateId",
                table: "ShippingCompanyGovernorateDetails");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "ShippingCompanyGovernorateDetails");

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
