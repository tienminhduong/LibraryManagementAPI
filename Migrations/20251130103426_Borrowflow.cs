using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class Borrowflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BorrowRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    QrCode = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowRequests_MemberInfos_MemberId",
                        column: x => x.MemberId,
                        principalTable: "MemberInfos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowRequests_StaffInfos_StaffId",
                        column: x => x.StaffId,
                        principalTable: "StaffInfos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "BorrowRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BorrowRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookCopyId = table.Column<Guid>(type: "uuid", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_MemberId",
                table: "BorrowRequests",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_StaffId",
                table: "BorrowRequests",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowRequestItems");

            migrationBuilder.DropTable(
                name: "BorrowRequests");
        }
    }
}
