using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplestTwilio.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageAndHistoryModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now')");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "Histories",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedSends",
                table: "Histories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDate",
                table: "Histories",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now')");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Histories",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "Completed");

            migrationBuilder.AddColumn<int>(
                name: "SuccessfulSends",
                table: "Histories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRecipients",
                table: "Histories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Histories_MessageId",
                table: "Histories",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_RecipientListId",
                table: "Histories",
                column: "RecipientListId");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_SentDate",
                table: "Histories",
                column: "SentDate");

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_Messages_MessageId",
                table: "Histories",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_RecipientLists_RecipientListId",
                table: "Histories",
                column: "RecipientListId",
                principalTable: "RecipientLists",
                principalColumn: "RecipientListId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Histories_Messages_MessageId",
                table: "Histories");

            migrationBuilder.DropForeignKey(
                name: "FK_Histories_RecipientLists_RecipientListId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_MessageId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_RecipientListId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_SentDate",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "FailedSends",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "SentDate",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "SuccessfulSends",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "TotalRecipients",
                table: "Histories");
        }
    }
}
