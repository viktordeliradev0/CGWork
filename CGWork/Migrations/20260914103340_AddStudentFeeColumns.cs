using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CGWork.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentFeeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FeePaidDate",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeePaid",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeePaidDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsFeePaid",
                table: "Students");
        }
    }
}
