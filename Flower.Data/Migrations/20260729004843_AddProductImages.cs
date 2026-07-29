using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flower.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Original migration was generated SQL Server -> PostgreSQL type conversions
            // but crashed mid-apply, dropping 4 indexes below before it could finish.
            // Re-create those indexes + create the ProductImages table.
            // All operations use IF NOT EXISTS / CREATE IF NOT EXISTS for idempotency.

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
            // Irreversible — column-type conversions were SQL Server specific.
        }
    }
}
