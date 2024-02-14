using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class paymentsalary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_StoreWallets_StoreWalletId",
                table: "SalaryPayments");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "SalaryPayments");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "SalaryPayments");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "SalaryPayments",
                newName: "Notes");

            migrationBuilder.AlterColumn<int>(
                name: "StoreWalletId",
                table: "SalaryPayments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Deduction",
                table: "SalaryPayments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Increase",
                table: "SalaryPayments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "SalaryPayments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "SalaryPayments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SalaryPayments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_StoreWallets_StoreWalletId",
                table: "SalaryPayments",
                column: "StoreWalletId",
                principalTable: "StoreWallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_StoreWallets_StoreWalletId",
                table: "SalaryPayments");

            migrationBuilder.DropColumn(
                name: "Deduction",
                table: "SalaryPayments");

            migrationBuilder.DropColumn(
                name: "Increase",
                table: "SalaryPayments");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "SalaryPayments");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "SalaryPayments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SalaryPayments");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "SalaryPayments",
                newName: "Note");

            migrationBuilder.AlterColumn<int>(
                name: "StoreWalletId",
                table: "SalaryPayments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "SalaryPayments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "SalaryPayments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_StoreWallets_StoreWalletId",
                table: "SalaryPayments",
                column: "StoreWalletId",
                principalTable: "StoreWallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
