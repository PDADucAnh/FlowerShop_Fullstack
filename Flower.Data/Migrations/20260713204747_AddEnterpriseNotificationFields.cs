using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flower.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnterpriseNotificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavigationUrl",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Notifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "AdminNotifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "AdminNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavigationUrl",
                table: "AdminNotifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "AdminNotifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "AdminNotifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "AdminNotifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NavigationUrl",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "AdminNotifications");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "AdminNotifications");

            migrationBuilder.DropColumn(
                name: "NavigationUrl",
                table: "AdminNotifications");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "AdminNotifications");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "AdminNotifications");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "AdminNotifications");
        }
    }
}
