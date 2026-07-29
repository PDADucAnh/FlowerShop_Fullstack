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
            // No-op: this migration was generated for SQL Server but the production
            // database is PostgreSQL. All column type conversions (nvarchar, datetime2, bit)
            // are incompatible with PostgreSQL.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
