using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class LinkedSpecializationAndImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecializationId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_SpecializationId",
                table: "Images",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Specializations_SpecializationId",
                table: "Images",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Specializations_SpecializationId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_SpecializationId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Images");
        }
    }
}
