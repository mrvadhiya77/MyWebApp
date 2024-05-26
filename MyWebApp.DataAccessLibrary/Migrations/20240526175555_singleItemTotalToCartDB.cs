using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWebApp.DataAccessLibrary.Migrations
{
    /// <inheritdoc />
    public partial class singleItemTotalToCartDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "singleItemTotal",
                table: "Carts",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "singleItemTotal",
                table: "Carts");
        }
    }
}
