using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedRelationshipsForRegionsAndCountriesAndGorenrates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GovernorateId",
                table: "Regions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Governments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_GovernorateId",
                table: "Regions",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Governments_CountryId",
                table: "Governments",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Governments_Countries_CountryId",
                table: "Governments",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Regions_Governments_GovernorateId",
                table: "Regions",
                column: "GovernorateId",
                principalTable: "Governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Governments_Countries_CountryId",
                table: "Governments");

            migrationBuilder.DropForeignKey(
                name: "FK_Regions_Governments_GovernorateId",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Regions_GovernorateId",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Governments_CountryId",
                table: "Governments");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Governments");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Countries");
        }
    }
}
