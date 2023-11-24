using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumnsInStoresTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Stores",
                newName: "EnglishName");

            migrationBuilder.AddColumn<bool>(
                name: "AcceptsReturns",
                table: "Stores",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Activity",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArabicName",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GovernorateId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NationalIdImageId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfileCoverImageId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureImageId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxRegistrationDocumentImageId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencySymbol = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Governments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GovernorateCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CountryId",
                table: "Stores",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_GovernorateId",
                table: "Stores",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_NationalIdImageId",
                table: "Stores",
                column: "NationalIdImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_ProfileCoverImageId",
                table: "Stores",
                column: "ProfileCoverImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_ProfilePictureImageId",
                table: "Stores",
                column: "ProfilePictureImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_RegionId",
                table: "Stores",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_TaxRegistrationDocumentImageId",
                table: "Stores",
                column: "TaxRegistrationDocumentImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Username",
                table: "Stores",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_StoreId",
                table: "Categories",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Stores_StoreId",
                table: "Categories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Countries_CountryId",
                table: "Stores",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Governments_GovernorateId",
                table: "Stores",
                column: "GovernorateId",
                principalTable: "Governments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Images_NationalIdImageId",
                table: "Stores",
                column: "NationalIdImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Images_ProfileCoverImageId",
                table: "Stores",
                column: "ProfileCoverImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Images_ProfilePictureImageId",
                table: "Stores",
                column: "ProfilePictureImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Images_TaxRegistrationDocumentImageId",
                table: "Stores",
                column: "TaxRegistrationDocumentImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Regions_RegionId",
                table: "Stores",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Stores_StoreId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Countries_CountryId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Governments_GovernorateId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Images_NationalIdImageId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Images_ProfileCoverImageId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Images_ProfilePictureImageId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Images_TaxRegistrationDocumentImageId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Regions_RegionId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Governments");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Stores_CountryId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_GovernorateId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_NationalIdImageId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_ProfileCoverImageId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_ProfilePictureImageId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_RegionId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_TaxRegistrationDocumentImageId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_Username",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Categories_StoreId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AcceptsReturns",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Activity",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "ArabicName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "NationalIdImageId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "ProfileCoverImageId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "ProfilePictureImageId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "TaxRegistrationDocumentImageId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "EnglishName",
                table: "Stores",
                newName: "Name");
        }
    }
}
