using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPictureToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrls",
                table: "Posts");

            migrationBuilder.CreateTable(
                name: "PictureUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PictureUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PictureUrls_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "9fa8b20b-578c-4850-83a3-da205a8d61fa", new DateTime(2025, 1, 6, 13, 40, 6, 983, DateTimeKind.Local).AddTicks(1626), "165b9ef6-07df-41ea-b566-cbffbfda344d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "13055474-db0c-40ba-bed5-2fdb7195d82e", new DateTime(2025, 1, 6, 13, 40, 6, 983, DateTimeKind.Local).AddTicks(1725), "74a5b6d7-b30b-4155-8c61-dc9dd12c7c1b" });

            migrationBuilder.CreateIndex(
                name: "IX_PictureUrls_PostId",
                table: "PictureUrls",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PictureUrls");

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
    }
}
