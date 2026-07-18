using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mydata.Migrations
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckOutDate",
                table: "BookingDetails",
                newName: "ExpectedCheckOutDate");

            migrationBuilder.RenameColumn(
                name: "CheckInDate",
                table: "BookingDetails",
                newName: "ExpectedCheckInDate");

            migrationBuilder.AddColumn<string>(
                name: "StatusCheckRoom",
                table: "BookingDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCheckRoom",
                table: "BookingDetails");

            migrationBuilder.RenameColumn(
                name: "ExpectedCheckOutDate",
                table: "BookingDetails",
                newName: "CheckOutDate");

            migrationBuilder.RenameColumn(
                name: "ExpectedCheckInDate",
                table: "BookingDetails",
                newName: "CheckInDate");
        }
    }
}
