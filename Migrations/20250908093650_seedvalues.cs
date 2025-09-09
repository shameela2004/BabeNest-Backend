using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BabeNest_Backend.Migrations
{
    /// <inheritdoc />
    public partial class seedvalues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Image", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, 1, "Gentle shampoo for babies", "images/products/baby-shampoo.jpg", "Baby Shampoo", 199.99m, 50 },
                    { 2, 2, "Fluffy teddy bear toy for infants", "images/products/teddy-bear.jpg", "Soft Teddy Bear", 499.00m, 20 },
                    { 3, 3, "Cotton onesie for newborns", "images/products/baby-onesie.jpg", "Baby Onesie", 299.50m, 100 },
                    { 4, 4, "Anti-colic feeding bottle", "images/products/feeding-bottle.jpg", "Feeding Bottle", 150.00m, 75 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

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
        }
    }
}
