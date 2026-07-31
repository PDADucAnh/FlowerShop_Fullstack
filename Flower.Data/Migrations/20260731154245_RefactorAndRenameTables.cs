using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flower.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAndRenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "PostCategories");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Posts",
                newName: "PostCategoryId");

            RenameForeignKey(
                migrationBuilder,
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts",
                newName: "FK_Posts_PostCategories_PostCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                newName: "IX_Posts_PostCategoryId");

            migrationBuilder.RenameTable(
                name: "CategoriesProducts",
                newName: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "CategoryProductId",
                table: "Products",
                newName: "ProductCategoryId");

            RenameForeignKey(
                migrationBuilder,
                name: "FK_Products_CategoriesProducts_CategoryProductId",
                table: "Products",
                newName: "FK_Products_ProductCategories_ProductCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryProductId",
                table: "Products",
                newName: "IX_Products_ProductCategoryId");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "CustomerNotifications");

            RenameForeignKey(
                migrationBuilder,
                name: "FK_Notifications_Customers_CustomerId",
                table: "CustomerNotifications",
                newName: "FK_CustomerNotifications_Customers_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_CustomerId",
                table: "CustomerNotifications",
                newName: "IX_CustomerNotifications_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_CustomerId_IsRead",
                table: "CustomerNotifications",
                newName: "IX_CustomerNotifications_CustomerId_IsRead");

            migrationBuilder.RenameColumn(
                name: "PriceAdjustment",
                table: "ProductVariants",
                newName: "Price");

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "ProductVariants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sku",
                table: "ProductVariants");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ProductVariants",
                newName: "PriceAdjustment");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerNotifications_CustomerId_IsRead",
                table: "CustomerNotifications",
                newName: "IX_Notifications_CustomerId_IsRead");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerNotifications_CustomerId",
                table: "CustomerNotifications",
                newName: "IX_Notifications_CustomerId");

            RenameForeignKey(
                migrationBuilder,
                name: "FK_CustomerNotifications_Customers_CustomerId",
                table: "CustomerNotifications",
                newName: "FK_Notifications_Customers_CustomerId");

            migrationBuilder.RenameTable(
                name: "CustomerNotifications",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products",
                newName: "IX_Products_CategoryProductId");

            RenameForeignKey(
                migrationBuilder,
                name: "FK_Products_ProductCategories_ProductCategoryId",
                table: "Products",
                newName: "FK_Products_CategoriesProducts_CategoryProductId");

            migrationBuilder.RenameColumn(
                name: "ProductCategoryId",
                table: "Products",
                newName: "CategoryProductId");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                newName: "CategoriesProducts");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_PostCategoryId",
                table: "Posts",
                newName: "IX_Posts_CategoryId");

            RenameForeignKey(
                migrationBuilder,
                name: "FK_Posts_PostCategories_PostCategoryId",
                table: "Posts",
                newName: "FK_Posts_Categories_CategoryId");

            migrationBuilder.RenameColumn(
                name: "PostCategoryId",
                table: "Posts",
                newName: "CategoryId");

            migrationBuilder.RenameTable(
                name: "PostCategories",
                newName: "Categories");
        }

        private static void RenameForeignKey(MigrationBuilder migrationBuilder, string name, string table, string newName)
        {
            migrationBuilder.Sql(
                migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL"
                    ? $"ALTER TABLE \"{table}\" RENAME CONSTRAINT \"{name}\" TO \"{newName}\";"
                    : $"EXEC sp_rename N'{name}', N'{newName}', N'OBJECT';",
                suppressTransaction: true);
        }
    }
}
