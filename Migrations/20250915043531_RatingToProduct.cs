﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BabeNest_Backend.Migrations
{
    /// <inheritdoc />
    public partial class RatingToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
      name: "Rating",
      table: "Products",
      type: "float",
      nullable: false,
      defaultValue: 0.0);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
       name: "Rating",
       table: "Products");

        }
    }
}
