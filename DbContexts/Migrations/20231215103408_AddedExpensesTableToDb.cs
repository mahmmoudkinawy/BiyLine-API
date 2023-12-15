using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedExpensesTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpenseId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseTypeId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    StoreWalletId = table.Column<int>(type: "int", nullable: false),
                    ReceiptImageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                        column: x => x.ExpenseTypeId,
                        principalTable: "ExpenseTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_Images_ReceiptImageId",
                        column: x => x.ReceiptImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_StoreWallets_StoreWalletId",
                        column: x => x.StoreWalletId,
                        principalTable: "StoreWallets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ExpenseId",
                table: "Images",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ReceiptImageId",
                table: "Expenses",
                column: "ReceiptImageId",
                unique: true,
                filter: "[ReceiptImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_StoreId",
                table: "Expenses",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_StoreWalletId",
                table: "Expenses",
                column: "StoreWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Expenses_ExpenseId",
                table: "Images",
                column: "ExpenseId",
                principalTable: "Expenses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Expenses_ExpenseId",
                table: "Images");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Images_ExpenseId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ExpenseId",
                table: "Images");
        }
    }
}
