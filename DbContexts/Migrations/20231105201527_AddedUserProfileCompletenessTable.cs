using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserProfileCompletenessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoresProfilesCompleteness",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDetailsComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsSpecializationsComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsCoverImageComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsProfileImageComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsTaxImageComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsNationalIdImageComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsAddressComplete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoresProfilesCompleteness", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoresProfilesCompleteness_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoresProfilesCompleteness_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoresProfilesCompleteness_StoreId",
                table: "StoresProfilesCompleteness",
                column: "StoreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoresProfilesCompleteness_UserId",
                table: "StoresProfilesCompleteness",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoresProfilesCompleteness");
        }
    }
}
