using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flower.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryImageAndAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "CategoriesProducts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "CategoriesProducts");
        }
    }
}
