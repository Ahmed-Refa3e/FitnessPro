using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeseedingdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Gyms",
                keyColumn: "GymID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Gyms",
                keyColumn: "GymID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AvailableForOnlineTraining", "Bio", "ConcurrencyStamp", "DateOfBirth", "Discriminator", "Email", "EmailConfirmed", "FirstName", "Gender", "JoinedDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "coach1", 0, true, null, "21dd1da4-0707-43dd-9402-8e4402d92947", new DateTime(1985, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Coach", "johndoe@example.com", false, "John", "Male", new DateTime(2025, 2, 25, 1, 59, 3, 747, DateTimeKind.Local).AddTicks(6361), "Doe", false, null, "JOHNDOE@EXAMPLE.COM", "JOHNDOE", null, "0123456789", false, null, "29d06c01-b71b-47ae-8230-147d8121e7cb", false, "johndoe" },
                    { "coach2", 0, false, null, "bee0806f-fa14-4b0a-b51d-3eb11c13ab15", new DateTime(1990, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Coach", "janesmith@example.com", false, "Jane", "Female", new DateTime(2025, 2, 25, 1, 59, 3, 747, DateTimeKind.Local).AddTicks(6502), "Smith", false, null, "JANESMITH@EXAMPLE.COM", "JANESMITH", null, "0987654321", false, null, "124b7601-cb7d-4165-adbe-fc0628717fe0", false, "janesmith" }
                });

            migrationBuilder.InsertData(
                table: "Gyms",
                columns: new[] { "GymID", "Address", "City", "CoachID", "Description", "FortnightlyPrice", "Governorate", "GymName", "MonthlyPrice", "PhoneNumber", "PictureUrl", "SessionPrice", "YearlyPrice" },
                values: new object[,]
                {
                    { 1, "123 Main St", "Tanta", "coach1", "A top-tier gym with all the modern equipment you need.", 30, "Gharbia", "Downtown Fitness", 50, "0123456789", null, 15, 500 },
                    { 2, "456 Sunset Blvd", "Zefta", "coach2", "A wellness center focused on body and mind fitness.", 25, "Gharbia", "Sunset Wellness", 40, "0987654321", null, 12, 450 }
                });
        }
    }
}
