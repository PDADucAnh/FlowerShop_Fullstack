using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flower.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnsureProductImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Prior migration AddProductImages was recorded as applied (no-op)
            // but never created the ProductImages table or re-created the dropped indexes.
            // This migration ensures those exist, idempotently.

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Products_Sku""
                ON ""Products"" (""Sku"")
                WHERE ""Sku"" IS NOT NULL
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_Orders_Status""
                ON ""Orders"" (""Status"")
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_Customers_ResetToken""
                ON ""Customers"" (""ResetToken"")
                WHERE ""ResetToken"" IS NOT NULL
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_CustomerAddresses_CustomerId_IsDefault""
                ON ""CustomerAddresses"" (""CustomerId"", ""IsDefault"")
                WHERE ""IsDefault"" = true
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""ProductImages"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""ProductId"" INTEGER NOT NULL,
                    ""ImageUrl"" VARCHAR(2000) NOT NULL,
                    ""SortOrder"" INTEGER NOT NULL DEFAULT 0,
                    ""CreatedAt"" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    CONSTRAINT ""FK_ProductImages_Products_ProductId""
                        FOREIGN KEY (""ProductId"")
                        REFERENCES ""Products""(""Id"")
                        ON DELETE CASCADE
                )
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_ProductImages_ProductId""
                ON ""ProductImages"" (""ProductId"")
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""ProductImages"" CASCADE");
        }
    }
}
