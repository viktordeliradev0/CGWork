using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CGWork.Migrations
{
    /// <inheritdoc />
    public partial class AddSemesters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_SemesterId",
                table: "Groups",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Semesters_SemesterId",
                table: "Groups",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Semesters_SemesterId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropIndex(
                name: "IX_Groups_SemesterId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Groups");
        }
    }
}
