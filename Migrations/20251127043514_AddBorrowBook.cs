using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "loginId",
                table: "StaffInfos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "loginId",
                table: "MemberInfos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "loginId",
                table: "AdminInfos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BookCopies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bookId = table.Column<Guid>(type: "uuid", nullable: false),
                    bookImportDetailId = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCopies", x => x.id);
                    table.ForeignKey(
                        name: "FK_BookCopies_Books_bookId",
                        column: x => x.bookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookTransactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    copyId = table.Column<Guid>(type: "uuid", nullable: false),
                    memberId = table.Column<Guid>(type: "uuid", nullable: false),
                    staffId = table.Column<Guid>(type: "uuid", nullable: false),
                    borrowDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    returnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookTransactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_BookTransactions_Books_copyId",
                        column: x => x.copyId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookTransactions_MemberInfos_memberId",
                        column: x => x.memberId,
                        principalTable: "MemberInfos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookTransactions_StaffInfos_staffId",
                        column: x => x.staffId,
                        principalTable: "StaffInfos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    phoneNumber = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BookImports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    staffId = table.Column<Guid>(type: "uuid", nullable: false),
                    importDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    totalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookImports", x => x.id);
                    table.ForeignKey(
                        name: "FK_BookImports_StaffInfos_staffId",
                        column: x => x.staffId,
                        principalTable: "StaffInfos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookImports_Supplier_supplierId",
                        column: x => x.supplierId,
                        principalTable: "Supplier",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookImportDetails",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bookImportId = table.Column<Guid>(type: "uuid", nullable: false),
                    bookId = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookImportDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_BookImportDetails_BookImports_bookImportId",
                        column: x => x.bookImportId,
                        principalTable: "BookImports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookImportDetails_Books_bookId",
                        column: x => x.bookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_bookId",
                table: "BookCopies",
                column: "bookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookImportDetails_bookId",
                table: "BookImportDetails",
                column: "bookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookImportDetails_bookImportId",
                table: "BookImportDetails",
                column: "bookImportId");

            migrationBuilder.CreateIndex(
                name: "IX_BookImports_staffId",
                table: "BookImports",
                column: "staffId");

            migrationBuilder.CreateIndex(
                name: "IX_BookImports_supplierId",
                table: "BookImports",
                column: "supplierId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTransactions_copyId",
                table: "BookTransactions",
                column: "copyId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTransactions_memberId",
                table: "BookTransactions",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTransactions_staffId",
                table: "BookTransactions",
                column: "staffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookCopies");

            migrationBuilder.DropTable(
                name: "BookImportDetails");

            migrationBuilder.DropTable(
                name: "BookTransactions");

            migrationBuilder.DropTable(
                name: "BookImports");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.AlterColumn<Guid>(
                name: "loginId",
                table: "StaffInfos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "loginId",
                table: "MemberInfos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "loginId",
                table: "AdminInfos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
