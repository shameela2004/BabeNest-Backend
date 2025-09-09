using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BabeNest_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Blocked", "CreatedAt", "Email", "PasswordHash", "Role", "Username" },
                values: new object[] { 1000, false, new DateTime(2025, 9, 8, 12, 0, 0, 0, DateTimeKind.Unspecified), "admin@babenest.com", "$2a$11$uJX3yK5E6M/T7bG.Z1Lr6e3N6W85i/7gpoZkiy3vMb/n1/0U4hYF2", "Admin", "AdminUser" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Baby Care" },
                    { 2, "Toys" },
                    { 3, "Clothing" },
                    { 4, "Feeding" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Blocked", "CreatedAt", "Email", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, false, new DateTime(2025, 9, 8, 12, 0, 0, 0, DateTimeKind.Unspecified), "admin@babenest.com", "$2a$11$nk60m0qnGbofWbvSJ8VaFOir0w9PrbJ0i/byW6/kLdVYT6ktIdFR2", "Admin", "Admin" });
        }
    }
}
