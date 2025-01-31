using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Adduserfollow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userFollows",
                columns: table => new
                {
                    FollowerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowingId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userFollows", x => new { x.FollowingId, x.FollowerId });
                    table.ForeignKey(
                        name: "FK_userFollows_AspNetUsers_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_userFollows_AspNetUsers_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "04313ca5-4b2a-47d0-b026-6853fb7f2a9a", new DateTime(2025, 1, 31, 18, 38, 50, 841, DateTimeKind.Local).AddTicks(7396), "13b70b77-35d1-4319-a5fe-8c65ecccabe3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "9b53944a-4fff-4a75-b320-644e93d42d91", new DateTime(2025, 1, 31, 18, 38, 50, 841, DateTimeKind.Local).AddTicks(7520), "488cf300-0006-4ecf-be0e-f1cfb0d4024c" });

            migrationBuilder.CreateIndex(
                name: "IX_userFollows_FollowerId",
                table: "userFollows",
                column: "FollowerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userFollows");

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
        }
    }
}
