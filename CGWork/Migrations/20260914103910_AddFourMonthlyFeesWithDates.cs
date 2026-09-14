using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CGWork.Migrations
{
    /// <inheritdoc />
    public partial class AddFourMonthlyFeesWithDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPaid",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsFeePaid",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "FeePaidDate",
                table: "Students",
                newName: "PaidMonth4Date");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidMonth1Date",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidMonth2Date",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidMonth3Date",
                table: "Students",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidMonth1Date",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PaidMonth2Date",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PaidMonth3Date",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "PaidMonth4Date",
                table: "Students",
                newName: "FeePaidDate");

            migrationBuilder.AddColumn<bool>(
                name: "HasPaid",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeePaid",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
