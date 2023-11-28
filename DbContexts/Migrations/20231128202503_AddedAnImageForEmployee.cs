using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedAnImageForEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ImageOwnerId",
                table: "Employees",
                column: "ImageOwnerId",
                unique: true,
                filter: "[ImageOwnerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Images_ImageOwnerId",
                table: "Employees",
                column: "ImageOwnerId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Images_ImageOwnerId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ImageOwnerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ImageOwnerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "Employees");
        }
    }
}
