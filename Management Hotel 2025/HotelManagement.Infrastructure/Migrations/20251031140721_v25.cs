using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mydata.Migrations
{
    /// <inheritdoc />
    public partial class v25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingDetailId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "BookingService",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdStaff = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deposit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_bookings_OrderId",
                        column: x => x.OrderId,
                        principalTable: "bookings",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_BookingDetailId",
                table: "Services",
                column: "BookingDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingService_OrderId",
                table: "BookingService",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingService_Order_OrderId",
                table: "BookingService",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_BookingDetails_BookingDetailId",
                table: "Services",
                column: "BookingDetailId",
                principalTable: "BookingDetails",
                principalColumn: "BookingDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingService_Order_OrderId",
                table: "BookingService");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_BookingDetails_BookingDetailId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Services_BookingDetailId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_BookingService_OrderId",
                table: "BookingService");

            migrationBuilder.DropColumn(
                name: "BookingDetailId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "BookingService");
        }
    }
}
