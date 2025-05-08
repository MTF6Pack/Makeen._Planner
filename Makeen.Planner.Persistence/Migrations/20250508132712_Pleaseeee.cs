using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Pleaseeee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old int ID column
            migrationBuilder.DropPrimaryKey(
                name: "PK_QueuedNotifications",
                table: "QueuedNotifications");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QueuedNotifications");

            // Add new Guid ID column
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "QueuedNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()"); // Use NEWID or NEWSEQUENTIALID if needed

            // Set as primary key again
            migrationBuilder.AddPrimaryKey(
                name: "PK_QueuedNotifications",
                table: "QueuedNotifications",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QueuedNotifications",
                table: "QueuedNotifications");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QueuedNotifications");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "QueuedNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueuedNotifications",
                table: "QueuedNotifications",
                column: "Id");
        }
    }
}
