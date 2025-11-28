using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixBookTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookTransactions_Books_copyId",
                table: "BookTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_BookTransactions_BookCopies_copyId",
                table: "BookTransactions",
                column: "copyId",
                principalTable: "BookCopies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookTransactions_BookCopies_copyId",
                table: "BookTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_BookTransactions_Books_copyId",
                table: "BookTransactions",
                column: "copyId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
