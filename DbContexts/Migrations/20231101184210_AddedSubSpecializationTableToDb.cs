using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubSpecializationTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Specializations_SpecializationId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_SpecializationId",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "SpecializationId",
                table: "Images",
                newName: "SubSpecializationId");

            migrationBuilder.CreateTable(
                name: "SubSpecializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecializationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubSpecializations_Images_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubSpecializations_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubSpecializations_SpecializationId",
                table: "SubSpecializations",
                column: "SpecializationId",
                unique: true,
                filter: "[SpecializationId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubSpecializations");

            migrationBuilder.RenameColumn(
                name: "SubSpecializationId",
                table: "Images",
                newName: "SpecializationId");

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
    }
}
