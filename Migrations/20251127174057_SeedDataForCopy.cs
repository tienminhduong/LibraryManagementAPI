using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataForCopy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BookCopies",
                columns: new[] { "id", "bookId", "bookImportDetailId", "status" },
                values: new object[,]
                {
                    { new Guid("00989681-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("00989682-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("00989683-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("00989684-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("00989685-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("00989686-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("00989687-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("00989688-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("00989689-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 },
                    { new Guid("0098968a-0000-0000-0000-000000000009"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("f0000000-0000-0000-0000-000000000007"), 0 }
                });

            migrationBuilder.InsertData(
                table: "Supplier",
                columns: new[] { "id", "address", "email", "name", "phoneNumber" },
                values: new object[] { new Guid("d0000000-0000-0000-0000-000000000004"), "", "", "Default Supplier", "" });

            migrationBuilder.InsertData(
                table: "BookImports",
                columns: new[] { "id", "importDate", "note", "staffId", "supplierId", "totalAmount" },
                values: new object[,]
                {
                    { new Guid("e0000000-0000-0000-0000-000000000005"), new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "First Import", new Guid("019aa216-657c-7b6d-abd5-6db1b06317ee"), new Guid("d0000000-0000-0000-0000-000000000004"), 0m },
                    { new Guid("e0000000-0000-0000-0000-000000000006"), new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Second Import", new Guid("019aa216-657c-7b6d-abd5-6db1b06317ee"), new Guid("d0000000-0000-0000-0000-000000000004"), 0m }
                });

            migrationBuilder.InsertData(
                table: "BookImportDetails",
                columns: new[] { "id", "bookId", "bookImportId", "quantity", "unitPrice" },
                values: new object[,]
                {
                    { new Guid("f0000000-0000-0000-0000-000000000007"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("e0000000-0000-0000-0000-000000000005"), 10, 100m },
                    { new Guid("f0000000-0000-0000-0000-000000000008"), new Guid("019a1a78-7b67-7f6c-9d63-f2d13554c669"), new Guid("e0000000-0000-0000-0000-000000000006"), 5, 200m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989681-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989682-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989683-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989684-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989685-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989686-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989687-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989688-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("00989689-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookCopies",
                keyColumn: "id",
                keyValue: new Guid("0098968a-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "BookImportDetails",
                keyColumn: "id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "BookImportDetails",
                keyColumn: "id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "BookImports",
                keyColumn: "id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "BookImports",
                keyColumn: "id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Supplier",
                keyColumn: "id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000004"));
        }
    }
}
