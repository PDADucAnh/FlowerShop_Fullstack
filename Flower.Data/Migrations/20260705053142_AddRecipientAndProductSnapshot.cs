using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flower.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipientAndProductSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Products_BasePrice ON Products;");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX IX_Products_BasePrice ON Products (Price) INCLUDE (Name, Slug, Sku, ImageUrl);");

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhone",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductImage",
                table: "OrderDetails",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SizeVariant",
                table: "OrderDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PriceAdjustment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Products_BasePrice ON Products;");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientPhone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "SizeVariant",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX IX_Products_BasePrice ON Products (Price) INCLUDE (Name, Slug, Sku, ImageUrl);");
        }
    }
}
