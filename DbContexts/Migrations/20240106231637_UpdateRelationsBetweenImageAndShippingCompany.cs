using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationsBetweenImageAndShippingCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Images_CommercialRegisterImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Images_IDImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_Images_ProfileImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropTable(
                name: "ShippingCompanyGovernorates");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_CommercialRegisterImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_IDImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_ProfileImageId",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "CommercialRegisterImageId",
                table: "ShippingCompanies");

            migrationBuilder.RenameColumn(
                name: "ProfileImageId",
                table: "ShippingCompanies",
                newName: "DeliveryCases");

            migrationBuilder.RenameColumn(
                name: "IDImageId",
                table: "ShippingCompanies",
                newName: "Collection");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ShippingCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "ShippingCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserEntityId",
                table: "ShippingCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippingCompanyEntityID",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShippingCompanyGovernorateDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PickUpCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    OverweightFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    ShippingCompanyId = table.Column<int>(type: "int", nullable: false),
                    GovernorateEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCompanyGovernorateDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingCompanyGovernorateDetails_Governments_GovernorateEntityId",
                        column: x => x.GovernorateEntityId,
                        principalTable: "Governments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShippingCompanyGovernorateDetails_ShippingCompanies_ShippingCompanyId",
                        column: x => x.ShippingCompanyId,
                        principalTable: "ShippingCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_UserEntityId",
                table: "ShippingCompanies",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ShippingCompanyEntityID",
                table: "Images",
                column: "ShippingCompanyEntityID");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorateDetails_GovernorateEntityId",
                table: "ShippingCompanyGovernorateDetails",
                column: "GovernorateEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorateDetails_ShippingCompanyId",
                table: "ShippingCompanyGovernorateDetails",
                column: "ShippingCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ShippingCompanies_ShippingCompanyEntityID",
                table: "Images",
                column: "ShippingCompanyEntityID",
                principalTable: "ShippingCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_AspNetUsers_UserEntityId",
                table: "ShippingCompanies",
                column: "UserEntityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ShippingCompanies_ShippingCompanyEntityID",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingCompanies_AspNetUsers_UserEntityId",
                table: "ShippingCompanies");

            migrationBuilder.DropTable(
                name: "ShippingCompanyGovernorateDetails");

            migrationBuilder.DropIndex(
                name: "IX_ShippingCompanies_UserEntityId",
                table: "ShippingCompanies");

            migrationBuilder.DropIndex(
                name: "IX_Images_ShippingCompanyEntityID",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "ShippingCompanies");

            migrationBuilder.DropColumn(
                name: "ShippingCompanyEntityID",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "DeliveryCases",
                table: "ShippingCompanies",
                newName: "ProfileImageId");

            migrationBuilder.RenameColumn(
                name: "Collection",
                table: "ShippingCompanies",
                newName: "IDImageId");

            migrationBuilder.AddColumn<int>(
                name: "CommercialRegisterImageId",
                table: "ShippingCompanies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShippingCompanyGovernorates",
                columns: table => new
                {
                    ShippingCompanyId = table.Column<int>(type: "int", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCompanyGovernorates", x => new { x.ShippingCompanyId, x.GovernorateId });
                    table.ForeignKey(
                        name: "FK_ShippingCompanyGovernorates_Governments_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingCompanyGovernorates_ShippingCompanies_ShippingCompanyId",
                        column: x => x.ShippingCompanyId,
                        principalTable: "ShippingCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_CommercialRegisterImageId",
                table: "ShippingCompanies",
                column: "CommercialRegisterImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_IDImageId",
                table: "ShippingCompanies",
                column: "IDImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanies_ProfileImageId",
                table: "ShippingCompanies",
                column: "ProfileImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCompanyGovernorates_GovernorateId",
                table: "ShippingCompanyGovernorates",
                column: "GovernorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Images_CommercialRegisterImageId",
                table: "ShippingCompanies",
                column: "CommercialRegisterImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Images_IDImageId",
                table: "ShippingCompanies",
                column: "IDImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingCompanies_Images_ProfileImageId",
                table: "ShippingCompanies",
                column: "ProfileImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }
    }
}
