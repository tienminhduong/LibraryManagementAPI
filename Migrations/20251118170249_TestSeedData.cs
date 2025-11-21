using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class TestSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "id", "createdAt", "isActive", "lastLogin", "passwordHash", "role", "userName" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000011"), new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "hashed_password1", 0, "admin" });

            migrationBuilder.InsertData(
                table: "AdminInfos",
                columns: new[] { "id", "email", "fullName", "loginId", "phoneNumber" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000111"), "admin@gmail.com", "Admin", new Guid("a0000000-0000-0000-0000-000000000011"), "0321547895" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AdminInfos",
                keyColumn: "id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000111"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000011"));
        }
    }
}
