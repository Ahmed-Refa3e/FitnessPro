using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addfollowgymandshop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gymFollows",
                columns: table => new
                {
                    FollowerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GymId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gymFollows", x => new { x.GymId, x.FollowerId });
                    table.ForeignKey(
                        name: "FK_gymFollows_AspNetUsers_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_gymFollows_Gyms_GymId",
                        column: x => x.GymId,
                        principalTable: "Gyms",
                        principalColumn: "GymID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopFollows",
                columns: table => new
                {
                    FollowerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopFollows", x => new { x.ShopId, x.FollowerId });
                    table.ForeignKey(
                        name: "FK_ShopFollows_AspNetUsers_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ShopFollows_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "98ee4d14-f7f0-4ccb-9e46-cc177501e3e8", new DateTime(2025, 1, 31, 19, 50, 46, 881, DateTimeKind.Local).AddTicks(3572), "58a37aee-ce88-4164-81ee-b9759a1d2c91" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "6576c74a-179f-41dd-b5d6-5f4b86a05a36", new DateTime(2025, 1, 31, 19, 50, 46, 881, DateTimeKind.Local).AddTicks(3698), "3607ac05-a524-40e2-a2af-5629898b7354" });

            migrationBuilder.CreateIndex(
                name: "IX_gymFollows_FollowerId",
                table: "gymFollows",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopFollows_FollowerId",
                table: "ShopFollows",
                column: "FollowerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gymFollows");

            migrationBuilder.DropTable(
                name: "ShopFollows");

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
        }
    }
}
