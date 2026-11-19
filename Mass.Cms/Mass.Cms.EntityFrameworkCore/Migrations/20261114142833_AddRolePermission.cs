using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mass.Cms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T1017_RoleInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T1017_RoleInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T1018_PermissionInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PermissionType = table.Column<int>(type: "int", nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PermissionDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T1018_PermissionInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_UserInfos_RoleInfos",
                columns: table => new
                {
                    RoleInfosId = table.Column<int>(type: "int", nullable: false),
                    UserInfosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserInfos_RoleInfos", x => new { x.RoleInfosId, x.UserInfosId });
                    table.ForeignKey(
                        name: "FK_T_UserInfos_RoleInfos_T1015_Users_UserInfosId",
                        column: x => x.UserInfosId,
                        principalTable: "T1015_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_UserInfos_RoleInfos_T1017_RoleInfos_RoleInfosId",
                        column: x => x.RoleInfosId,
                        principalTable: "T1017_RoleInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_RoleInfos_Permissions",
                columns: table => new
                {
                    PermissionInfosId = table.Column<int>(type: "int", nullable: false),
                    RoleInfosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_RoleInfos_Permissions", x => new { x.PermissionInfosId, x.RoleInfosId });
                    table.ForeignKey(
                        name: "FK_T_RoleInfos_Permissions_T1017_RoleInfos_RoleInfosId",
                        column: x => x.RoleInfosId,
                        principalTable: "T1017_RoleInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_RoleInfos_Permissions_T1018_PermissionInfos_PermissionInfosId",
                        column: x => x.PermissionInfosId,
                        principalTable: "T1018_PermissionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T1019_PermissionMenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuIcon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MenunOrder = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T1019_PermissionMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T1019_PermissionMenus_T1018_PermissionInfos_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "T1018_PermissionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T1020_PermissionPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PointIcon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PointStatus = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T1020_PermissionPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T1020_PermissionPoints_T1018_PermissionInfos_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "T1018_PermissionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T1021_PermissionApis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApiMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T1021_PermissionApis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T1021_PermissionApis_T1018_PermissionInfos_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "T1018_PermissionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_RoleInfos_Permissions_RoleInfosId",
                table: "T_RoleInfos_Permissions",
                column: "RoleInfosId");

            migrationBuilder.CreateIndex(
                name: "IX_T_UserInfos_RoleInfos_UserInfosId",
                table: "T_UserInfos_RoleInfos",
                column: "UserInfosId");

            migrationBuilder.CreateIndex(
                name: "IX_T1019_PermissionMenus_PermissionId",
                table: "T1019_PermissionMenus",
                column: "PermissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T1020_PermissionPoints_PermissionId",
                table: "T1020_PermissionPoints",
                column: "PermissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T1021_PermissionApis_PermissionId",
                table: "T1021_PermissionApis",
                column: "PermissionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_RoleInfos_Permissions");

            migrationBuilder.DropTable(
                name: "T_UserInfos_RoleInfos");

            migrationBuilder.DropTable(
                name: "T1019_PermissionMenus");

            migrationBuilder.DropTable(
                name: "T1020_PermissionPoints");

            migrationBuilder.DropTable(
                name: "T1021_PermissionApis");

            migrationBuilder.DropTable(
                name: "T1017_RoleInfos");

            migrationBuilder.DropTable(
                name: "T1018_PermissionInfos");
        }
    }
}
