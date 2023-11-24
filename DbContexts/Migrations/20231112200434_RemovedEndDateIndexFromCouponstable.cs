using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class RemovedEndDateIndexFromCouponstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code_EndDate",
                table: "Coupons");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code",
                table: "Coupons");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code_EndDate",
                table: "Coupons",
                columns: new[] { "Code", "EndDate" },
                unique: true,
                filter: "[Code] IS NOT NULL AND [EndDate] IS NOT NULL");
        }
    }
}
