using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mydata.Migrations
{
    /// <inheritdoc />
    public partial class v19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NamePassenger",
                table: "BookingDetails");

            migrationBuilder.DropColumn(
                name: "PersonalCodePassenger",
                table: "BookingDetails");

            migrationBuilder.DropColumn(
                name: "PhonePassenger",
                table: "BookingDetails");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "BookingDetails");

            migrationBuilder.AddColumn<int>(
                name: "AmountPersonal",
                table: "bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalCode",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sex",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPersonal",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "PersonalCode",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "bookings");

            migrationBuilder.AddColumn<string>(
                name: "NamePassenger",
                table: "BookingDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonalCodePassenger",
                table: "BookingDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhonePassenger",
                table: "BookingDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sex",
                table: "BookingDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
