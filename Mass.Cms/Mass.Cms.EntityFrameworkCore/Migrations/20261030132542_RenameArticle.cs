using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mass.Cms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class RenameArticle : Migration
    {
        /// <inheritdoc />
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Articels_T1015_Users_UserInfoId",
                table: "T_Articels");

            migrationBuilder.RenameTable(
                name: "T_Articels",
                newName: "T_Articles");

            migrationBuilder.RenameIndex(
                name: "IX_T_Articels_UserInfoId",
                table: "T_Articles",
                newName: "IX_T_Articles_UserInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Articles_T1015_Users_UserInfoId",
                table: "T_Articles",
                column: "UserInfoId",
                principalTable: "T1015_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Articles_T1015_Users_UserInfoId",
                table: "T_Articles");

            migrationBuilder.RenameTable(
                name: "T_Articles",
                newName: "T_Articels");

            migrationBuilder.RenameIndex(
                name: "IX_T_Articles_UserInfoId",
                table: "T_Articels",
                newName: "IX_T_Articels_UserInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Articels_T1015_Users_UserInfoId",
                table: "T_Articels",
                column: "UserInfoId",
                principalTable: "T1015_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
