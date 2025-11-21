using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccountAndInfoDataa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MemberInfos",
                keyColumn: "id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000033"),
                column: "joinDate",
                value: new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "StaffInfos",
                keyColumn: "id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000022"),
                column: "hireDate",
                value: new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MemberInfos",
                keyColumn: "id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000033"),
                column: "joinDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "StaffInfos",
                keyColumn: "id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000022"),
                column: "hireDate",
                value: new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
