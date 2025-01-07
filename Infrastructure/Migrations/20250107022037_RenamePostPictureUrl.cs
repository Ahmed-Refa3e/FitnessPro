using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePostPictureUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PictureUrls");

            migrationBuilder.CreateTable(
                name: "PostPictureUrl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostPictureUrl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostPictureUrl_Posts_PostId",
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
                values: new object[] { "9186e99f-4ea8-4bfb-8174-741d90b24ad7", new DateTime(2025, 1, 6, 21, 20, 35, 354, DateTimeKind.Local).AddTicks(6427), "4089b9dc-e74c-4281-a6ab-66d88ef6bfcc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "9d4cc175-d11e-4f4a-9b36-e0910cf2bec0", new DateTime(2025, 1, 6, 21, 20, 35, 354, DateTimeKind.Local).AddTicks(6537), "f8f929f6-4efc-47d6-92c5-322c3ed328c7" });

            migrationBuilder.CreateIndex(
                name: "IX_PostPictureUrl_PostId",
                table: "PostPictureUrl",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostPictureUrl");

            migrationBuilder.CreateTable(
                name: "PictureUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
    }
}
