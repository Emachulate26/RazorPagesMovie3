using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesMovie.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsForBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "MovieTimeSlot",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Movie",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Booking",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<int>(
                name: "ScreenNumber",
                table: "MovieTimeSlot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "MovieTimeSlot",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Movie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScreenNumber",
                table: "MovieTimeSlot");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "MovieTimeSlot");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Movie");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "MovieTimeSlot",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Movie",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Booking",
                newName: "Price");
        }
    }
}
