using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedAverageRatingAndDateAddedIndexesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_AverageRating",
                table: "Products",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DateAdded",
                table: "Products",
                column: "DateAdded");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_AverageRating",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_DateAdded",
                table: "Products");
        }
    }
}
