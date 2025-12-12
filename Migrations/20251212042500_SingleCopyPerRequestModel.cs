using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class SingleCopyPerRequestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop the BorrowRequestItems table (if it exists)
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS ""BorrowRequestItems"" CASCADE;
            ");

            // Step 2: Add new columns to BorrowRequests (nullable first)
            migrationBuilder.AddColumn<Guid>(
                name: "BookId",
                table: "BorrowRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BookCopyId",
                table: "BorrowRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowDate",
                table: "BorrowRequests",
                type: "timestamp with time zone",
                nullable: true);

            // Step 3: Delete all existing BorrowRequests from old structure
            // This is necessary because we can't migrate the old data structure
            migrationBuilder.Sql(@"
                DELETE FROM ""BorrowRequests"";
            ");

            // Step 4: Make BookId NOT NULL (safe now because table is empty)
            migrationBuilder.AlterColumn<Guid>(
                name: "BookId",
                table: "BorrowRequests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            // Step 5: Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_BookId",
                table: "BorrowRequests",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_BookCopyId",
                table: "BorrowRequests",
                column: "BookCopyId");

            // Step 6: Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRequests_BookCopies_BookCopyId",
                table: "BorrowRequests",
                column: "BookCopyId",
                principalTable: "BookCopies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_BookCopies_BookCopyId",
                table: "BorrowRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_Books_BookId",
                table: "BorrowRequests");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_BorrowRequests_BookCopyId",
                table: "BorrowRequests");

            migrationBuilder.DropIndex(
                name: "IX_BorrowRequests_BookId",
                table: "BorrowRequests");

            // Drop columns
            migrationBuilder.DropColumn(
                name: "BorrowDate",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "BookCopyId",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BorrowRequests");

            // Recreate BorrowRequestItems table
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
        }
    }
}
