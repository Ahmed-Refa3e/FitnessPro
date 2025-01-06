using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrls",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "99a1ba57-aaec-4781-95b5-57b8d389970e", new DateTime(2024, 12, 31, 14, 36, 4, 435, DateTimeKind.Local).AddTicks(2708), "cd8b8600-85b4-4e37-93ac-39180f4b76ad" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "3d49598a-aa1e-4b8d-8fc5-41586ba4b387", new DateTime(2024, 12, 31, 14, 36, 4, 435, DateTimeKind.Local).AddTicks(2802), "093e0e64-907e-4882-a048-f48fca5c94a8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrls",
                table: "Posts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "84a09b7b-f8b7-412d-86ad-9aff9ff73d85", new DateTime(2024, 12, 31, 9, 26, 32, 385, DateTimeKind.Local).AddTicks(4213), "12f98f02-d89e-4507-a214-799e574535e7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "5680d4fd-1f12-45a8-b8bf-2b27c3741d36", new DateTime(2024, 12, 31, 9, 26, 32, 385, DateTimeKind.Local).AddTicks(4307), "77993549-b593-440b-976e-acf821c47951" });
        }
    }
}
