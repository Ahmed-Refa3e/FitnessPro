using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCountryToGovernorate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Gyms",
                newName: "governorate");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "6feec2fa-a595-45c8-a145-6d3c178a969f", new DateTime(2024, 10, 25, 4, 16, 36, 635, DateTimeKind.Local).AddTicks(6409), "3894c18d-4c56-42db-a162-15417c4c2a82" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "28342731-ac5d-4925-9cd6-7d9877671889", new DateTime(2024, 10, 25, 4, 16, 36, 635, DateTimeKind.Local).AddTicks(6583), "218bd0a1-34d3-47ed-b0dd-e49d8bbf8819" });

            migrationBuilder.UpdateData(
                table: "Gyms",
                keyColumn: "GymID",
                keyValue: 1,
                columns: new[] { "City", "governorate" },
                values: new object[] { "Tanta", "Gharbia" });

            migrationBuilder.UpdateData(
                table: "Gyms",
                keyColumn: "GymID",
                keyValue: 2,
                columns: new[] { "City", "governorate" },
                values: new object[] { "Zefta", "Gharbia" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "governorate",
                table: "Gyms",
                newName: "Country");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "bfd3ee03-b32e-415e-aa9a-c0708d23777b", new DateTime(2024, 10, 24, 19, 16, 28, 947, DateTimeKind.Local).AddTicks(5263), "667b94b2-69f4-4c21-99b4-44af0df05ed7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "3965f34e-157b-4715-86ba-aea99190f6f9", new DateTime(2024, 10, 24, 19, 16, 28, 947, DateTimeKind.Local).AddTicks(5461), "29b81412-7d01-4d44-a5f8-32b867900d37" });

            migrationBuilder.UpdateData(
                table: "Gyms",
                keyColumn: "GymID",
                keyValue: 1,
                columns: new[] { "City", "Country" },
                values: new object[] { "Cairo", "Egypt" });

            migrationBuilder.UpdateData(
                table: "Gyms",
                keyColumn: "GymID",
                keyValue: 2,
                columns: new[] { "City", "Country" },
                values: new object[] { "Alexandria", "Egypt" });
        }
    }
}
