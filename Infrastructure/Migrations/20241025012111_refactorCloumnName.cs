using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class refactorCloumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "governorate",
                table: "Gyms",
                newName: "Governorate");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "d186acba-884c-47cc-8486-e6eb511519a4", new DateTime(2024, 10, 25, 4, 21, 11, 23, DateTimeKind.Local).AddTicks(3520), "46b59ba0-54c3-425b-98a5-9f8f26394382" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "43e7f43a-86dc-4254-a8f9-0c56cb3586bd", new DateTime(2024, 10, 25, 4, 21, 11, 23, DateTimeKind.Local).AddTicks(3671), "804e1a78-948d-4e4b-8ce8-fbbbadf5e2ab" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Governorate",
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
        }
    }
}
