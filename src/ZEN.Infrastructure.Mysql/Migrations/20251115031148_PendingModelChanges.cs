using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AspNetUsers",
                newName: "username");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_username_change_date",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "username_changed_count",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d3c1945f-a4e0-470b-aabd-88e81fb2a1b6",
                columns: new[] { "UserName", "is_public", "last_username_change_date", "slug", "username", "username_changed_count" },
                values: new object[] { "trunghuy", true, null, null, null, 0 });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
                columns: new[] { "UserName", "is_public", "last_username_change_date", "slug", "username", "username_changed_count" },
                values: new object[] { "trungthanh", true, null, null, null, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_username",
                table: "AspNetUsers",
                column: "username",
                unique: true,
                filter: "\"username\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_username_is_public",
                table: "AspNetUsers",
                columns: new[] { "username", "is_public" },
                filter: "\"username\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_username",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_username_is_public",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "is_public",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "last_username_change_date",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "username_changed_count",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "AspNetUsers",
                newName: "UserName");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d3c1945f-a4e0-470b-aabd-88e81fb2a1b6",
                column: "UserName",
                value: "trunghuy");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
                column: "UserName",
                value: "trungthanh");
        }
    }
}
