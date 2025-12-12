using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToSingleCopyPerRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowRequestItems");

            migrationBuilder.AddColumn<Guid>(
                name: "BookCopyId",
                table: "BorrowRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BookId",
                table: "BorrowRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowDate",
                table: "BorrowRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_BookCopyId",
                table: "BorrowRequests",
                column: "BookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_BookId",
                table: "BorrowRequests",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRequests_BookCopies_BookCopyId",
                table: "BorrowRequests",
                column: "BookCopyId",
                principalTable: "BookCopies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRequests_Books_BookId",
                table: "BorrowRequests",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_BookCopies_BookCopyId",
                table: "BorrowRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_Books_BookId",
                table: "BorrowRequests");

            migrationBuilder.DropIndex(
                name: "IX_BorrowRequests_BookCopyId",
                table: "BorrowRequests");

            migrationBuilder.DropIndex(
                name: "IX_BorrowRequests_BookId",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "BookCopyId",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "BorrowDate",
                table: "BorrowRequests");

            migrationBuilder.CreateTable(
                name: "BorrowRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookCopyId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    BorrowRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowRequestItems_BookCopies_BookCopyId",
                        column: x => x.BookCopyId,
                        principalTable: "BookCopies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_BorrowRequestItems_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowRequestItems_BorrowRequests_BorrowRequestId",
                        column: x => x.BorrowRequestId,
                        principalTable: "BorrowRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequestItems_BookCopyId",
                table: "BorrowRequestItems",
                column: "BookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequestItems_BookId",
                table: "BorrowRequestItems",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequestItems_BorrowRequestId",
                table: "BorrowRequestItems",
                column: "BorrowRequestId");
        }
    }
}
