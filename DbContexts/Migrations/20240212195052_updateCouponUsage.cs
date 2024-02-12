using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class updateCouponUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "CouponsUsages");

            migrationBuilder.AddColumn<int>(
                name: "ItemCount",
                table: "CouponsUsages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemCount",
                table: "CouponsUsages");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "CouponsUsages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
