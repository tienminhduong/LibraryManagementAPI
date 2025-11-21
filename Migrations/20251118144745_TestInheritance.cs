using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class TestInheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberInfo_Accounts_loginId",
                table: "MemberInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffInfo_Accounts_loginId",
                table: "StaffInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffInfo",
                table: "StaffInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberInfo",
                table: "MemberInfo");

            migrationBuilder.RenameTable(
                name: "StaffInfo",
                newName: "StaffInfos");

            migrationBuilder.RenameTable(
                name: "MemberInfo",
                newName: "MemberInfos");

            migrationBuilder.RenameIndex(
                name: "IX_StaffInfo_loginId",
                table: "StaffInfos",
                newName: "IX_StaffInfos_loginId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberInfo_loginId",
                table: "MemberInfos",
                newName: "IX_MemberInfos_loginId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffInfos",
                table: "StaffInfos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberInfos",
                table: "MemberInfos",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInfos_Accounts_loginId",
                table: "MemberInfos",
                column: "loginId",
                principalTable: "Accounts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffInfos_Accounts_loginId",
                table: "StaffInfos",
                column: "loginId",
                principalTable: "Accounts",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberInfos_Accounts_loginId",
                table: "MemberInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffInfos_Accounts_loginId",
                table: "StaffInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffInfos",
                table: "StaffInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberInfos",
                table: "MemberInfos");

            migrationBuilder.RenameTable(
                name: "StaffInfos",
                newName: "StaffInfo");

            migrationBuilder.RenameTable(
                name: "MemberInfos",
                newName: "MemberInfo");

            migrationBuilder.RenameIndex(
                name: "IX_StaffInfos_loginId",
                table: "StaffInfo",
                newName: "IX_StaffInfo_loginId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberInfos_loginId",
                table: "MemberInfo",
                newName: "IX_MemberInfo_loginId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffInfo",
                table: "StaffInfo",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberInfo",
                table: "MemberInfo",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInfo_Accounts_loginId",
                table: "MemberInfo",
                column: "loginId",
                principalTable: "Accounts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffInfo_Accounts_loginId",
                table: "StaffInfo",
                column: "loginId",
                principalTable: "Accounts",
                principalColumn: "id");
        }
    }
}
