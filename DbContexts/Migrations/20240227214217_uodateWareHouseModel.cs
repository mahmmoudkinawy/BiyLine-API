using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class uodateWareHouseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "WarehouseLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "Code",
                table: "WarehouseLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "OperationDate",
                table: "WarehouseLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "SellingPrice",
                table: "WarehouseLogs",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "WarehouseLogs");

            migrationBuilder.DropColumn(
                name: "OperationDate",
                table: "WarehouseLogs");

            migrationBuilder.DropColumn(
                name: "SellingPrice",
                table: "WarehouseLogs");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "WarehouseLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
