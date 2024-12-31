using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifyShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Shops",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Shops",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoachID",
                table: "Shops",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shops",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Governorate",
                table: "Shops",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Shops",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Shops",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Shops",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Shops_CoachID",
                table: "Shops",
                column: "CoachID");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_AspNetUsers_CoachID",
                table: "Shops",
                column: "CoachID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_AspNetUsers_CoachID",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_CoachID",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "CoachID",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Shops");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "8b2e651d-3256-454c-953d-d1ddab07311b", new DateTime(2024, 12, 25, 14, 53, 19, 35, DateTimeKind.Local).AddTicks(7617), "f6702139-ae15-4711-b315-c12ca752119d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "5f688165-ff1a-40fe-bdae-7cd2bd653c61", new DateTime(2024, 12, 25, 14, 53, 19, 35, DateTimeKind.Local).AddTicks(7729), "2eb31271-86a3-43da-94da-20a41fd0029c" });
        }
    }
}
