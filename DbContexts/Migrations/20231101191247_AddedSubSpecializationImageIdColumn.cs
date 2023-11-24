using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubSpecializationImageIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubSpecializations_Images_SpecializationId",
                table: "SubSpecializations");

            migrationBuilder.DropIndex(
                name: "IX_SubSpecializations_SpecializationId",
                table: "SubSpecializations");

            migrationBuilder.AddColumn<int>(
                name: "SubSpecializationImageId",
                table: "SubSpecializations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubSpecializations_SpecializationId",
                table: "SubSpecializations",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_SubSpecializations_SubSpecializationImageId",
                table: "SubSpecializations",
                column: "SubSpecializationImageId",
                unique: true,
                filter: "[SubSpecializationImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SubSpecializations_Images_SubSpecializationImageId",
                table: "SubSpecializations",
                column: "SubSpecializationImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubSpecializations_Images_SubSpecializationImageId",
                table: "SubSpecializations");

            migrationBuilder.DropIndex(
                name: "IX_SubSpecializations_SpecializationId",
                table: "SubSpecializations");

            migrationBuilder.DropIndex(
                name: "IX_SubSpecializations_SubSpecializationImageId",
                table: "SubSpecializations");

            migrationBuilder.DropColumn(
                name: "SubSpecializationImageId",
                table: "SubSpecializations");

            migrationBuilder.CreateIndex(
                name: "IX_SubSpecializations_SpecializationId",
                table: "SubSpecializations",
                column: "SpecializationId",
                unique: true,
                filter: "[SpecializationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SubSpecializations_Images_SpecializationId",
                table: "SubSpecializations",
                column: "SpecializationId",
                principalTable: "Images",
                principalColumn: "Id");
        }
    }
}
