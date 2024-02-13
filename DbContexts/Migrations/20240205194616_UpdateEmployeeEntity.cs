using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Images_ImageOwnerId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Images_NationalIdImageId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ImageOwnerId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_NationalIdImageId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ImageOwnerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "NationalIdImageId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PaymentPeriod",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "VisaNumber",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageOwnerId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "Employees",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NationalIdImageId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentPeriod",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisaNumber",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingHours",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ImageOwnerId",
                table: "Employees",
                column: "ImageOwnerId",
                unique: true,
                filter: "[ImageOwnerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NationalIdImageId",
                table: "Employees",
                column: "NationalIdImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Images_ImageOwnerId",
                table: "Employees",
                column: "ImageOwnerId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Images_NationalIdImageId",
                table: "Employees",
                column: "NationalIdImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }
    }
}
