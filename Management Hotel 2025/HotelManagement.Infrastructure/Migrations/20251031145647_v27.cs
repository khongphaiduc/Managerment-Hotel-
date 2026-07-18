using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mydata.Migrations
{
    /// <inheritdoc />
    public partial class v27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_BookingDetails_BookingDetailId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_BookingDetailId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "BookingDetailId",
                table: "Services");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingDetailId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_BookingDetailId",
                table: "Services",
                column: "BookingDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_BookingDetails_BookingDetailId",
                table: "Services",
                column: "BookingDetailId",
                principalTable: "BookingDetails",
                principalColumn: "BookingDetailId");
        }
    }
}
