using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mass.Cms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T1016_Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartmentDescr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T1016_Departments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T1016_Departments");
        }
    }
}
