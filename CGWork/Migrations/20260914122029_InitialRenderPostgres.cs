using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CGWork.Migrations
{
    /// <inheritdoc />
    public partial class InitialRenderPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonSessionStudent_LessonSessions_AttendedLessonsId",
                table: "LessonSessionStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonSessionStudent_Students_PresentStudentsId",
                table: "LessonSessionStudent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonSessionStudent",
                table: "LessonSessionStudent");

            migrationBuilder.RenameTable(
                name: "LessonSessionStudent",
                newName: "StudentLessonAttendances");

            migrationBuilder.RenameColumn(
                name: "PresentStudentsId",
                table: "StudentLessonAttendances",
                newName: "AttendingStudentsId");

            migrationBuilder.RenameIndex(
                name: "IX_LessonSessionStudent_PresentStudentsId",
                table: "StudentLessonAttendances",
                newName: "IX_StudentLessonAttendances_AttendingStudentsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLessonAttendances",
                table: "StudentLessonAttendances",
                columns: new[] { "AttendedLessonsId", "AttendingStudentsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonAttendances_LessonSessions_AttendedLessonsId",
                table: "StudentLessonAttendances",
                column: "AttendedLessonsId",
                principalTable: "LessonSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonAttendances_Students_AttendingStudentsId",
                table: "StudentLessonAttendances",
                column: "AttendingStudentsId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonAttendances_LessonSessions_AttendedLessonsId",
                table: "StudentLessonAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonAttendances_Students_AttendingStudentsId",
                table: "StudentLessonAttendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLessonAttendances",
                table: "StudentLessonAttendances");

            migrationBuilder.RenameTable(
                name: "StudentLessonAttendances",
                newName: "LessonSessionStudent");

            migrationBuilder.RenameColumn(
                name: "AttendingStudentsId",
                table: "LessonSessionStudent",
                newName: "PresentStudentsId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLessonAttendances_AttendingStudentsId",
                table: "LessonSessionStudent",
                newName: "IX_LessonSessionStudent_PresentStudentsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonSessionStudent",
                table: "LessonSessionStudent",
                columns: new[] { "AttendedLessonsId", "PresentStudentsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSessionStudent_LessonSessions_AttendedLessonsId",
                table: "LessonSessionStudent",
                column: "AttendedLessonsId",
                principalTable: "LessonSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSessionStudent_Students_PresentStudentsId",
                table: "LessonSessionStudent",
                column: "PresentStudentsId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
