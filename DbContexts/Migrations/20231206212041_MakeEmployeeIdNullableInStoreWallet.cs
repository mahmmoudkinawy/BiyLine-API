using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmployeeIdNullableInStoreWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreWallets_Employees_EmployeeId",
                table: "StoreWallets");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "StoreWallets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreWallets_Employees_EmployeeId",
                table: "StoreWallets",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreWallets_Employees_EmployeeId",
                table: "StoreWallets");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "StoreWallets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreWallets_Employees_EmployeeId",
                table: "StoreWallets",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
