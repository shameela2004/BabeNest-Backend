using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BabeNest_Backend.Migrations
{
    /// <inheritdoc />
    public partial class newtablesforordersStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Failed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Pending");
        }
    }
}
