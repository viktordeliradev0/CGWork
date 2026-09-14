using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CGWork.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "LessonSessions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "LessonSessions");
        }
    }
}
