using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiyLineApi.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedStoreMessageChatTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreChatMessages_Stores_StoreId",
                table: "StoreChatMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreChatMessages",
                table: "StoreChatMessages");

            migrationBuilder.DropColumn(
                name: "MessageText",
                table: "StoreChatMessages");

            migrationBuilder.DropColumn(
                name: "ReceiverUsername",
                table: "StoreChatMessages");

            migrationBuilder.RenameTable(
                name: "StoreChatMessages",
                newName: "StoreMessages");

            migrationBuilder.RenameColumn(
                name: "SenderUsername",
                table: "StoreMessages",
                newName: "Content");

            migrationBuilder.RenameIndex(
                name: "IX_StoreChatMessages_StoreId",
                table: "StoreMessages",
                newName: "IX_StoreMessages_StoreId");

            migrationBuilder.AddColumn<int>(
                name: "ReceiverUserId",
                table: "StoreMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SenderUserId",
                table: "StoreMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreMessages",
                table: "StoreMessages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StoreMessages_ReceiverUserId",
                table: "StoreMessages",
                column: "ReceiverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreMessages_SenderUserId",
                table: "StoreMessages",
                column: "SenderUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreMessages_AspNetUsers_ReceiverUserId",
                table: "StoreMessages",
                column: "ReceiverUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreMessages_AspNetUsers_SenderUserId",
                table: "StoreMessages",
                column: "SenderUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreMessages_Stores_StoreId",
                table: "StoreMessages",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreMessages_AspNetUsers_ReceiverUserId",
                table: "StoreMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreMessages_AspNetUsers_SenderUserId",
                table: "StoreMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreMessages_Stores_StoreId",
                table: "StoreMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreMessages",
                table: "StoreMessages");

            migrationBuilder.DropIndex(
                name: "IX_StoreMessages_ReceiverUserId",
                table: "StoreMessages");

            migrationBuilder.DropIndex(
                name: "IX_StoreMessages_SenderUserId",
                table: "StoreMessages");

            migrationBuilder.DropColumn(
                name: "ReceiverUserId",
                table: "StoreMessages");

            migrationBuilder.DropColumn(
                name: "SenderUserId",
                table: "StoreMessages");

            migrationBuilder.RenameTable(
                name: "StoreMessages",
                newName: "StoreChatMessages");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "StoreChatMessages",
                newName: "SenderUsername");

            migrationBuilder.RenameIndex(
                name: "IX_StoreMessages_StoreId",
                table: "StoreChatMessages",
                newName: "IX_StoreChatMessages_StoreId");

            migrationBuilder.AddColumn<string>(
                name: "MessageText",
                table: "StoreChatMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUsername",
                table: "StoreChatMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreChatMessages",
                table: "StoreChatMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreChatMessages_Stores_StoreId",
                table: "StoreChatMessages",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
