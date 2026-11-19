using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mass.Cms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class InitAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T1015_Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserPhone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PhotoUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "images/default.jpg"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T1015_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_Articels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserInfoId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Articels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Articels_T1015_Users_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "T1015_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Articels_UserInfoId",
                table: "T_Articels",
                column: "UserInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Articels");

            migrationBuilder.DropTable(
                name: "T1015_Users");
        }
    }
}
