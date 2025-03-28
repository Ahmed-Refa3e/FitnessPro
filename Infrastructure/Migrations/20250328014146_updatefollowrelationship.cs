using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatefollowrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gymFollows_AspNetUsers_FollowerId",
                table: "gymFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopFollows_AspNetUsers_FollowerId",
                table: "ShopFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_userFollows_AspNetUsers_FollowerId",
                table: "userFollows");

            migrationBuilder.AddForeignKey(
                name: "FK_gymFollows_AspNetUsers_FollowerId",
                table: "gymFollows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopFollows_AspNetUsers_FollowerId",
                table: "ShopFollows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userFollows_AspNetUsers_FollowerId",
                table: "userFollows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gymFollows_AspNetUsers_FollowerId",
                table: "gymFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopFollows_AspNetUsers_FollowerId",
                table: "ShopFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_userFollows_AspNetUsers_FollowerId",
                table: "userFollows");

            migrationBuilder.AddForeignKey(
                name: "FK_gymFollows_AspNetUsers_FollowerId",
                table: "gymFollows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopFollows_AspNetUsers_FollowerId",
                table: "ShopFollows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_userFollows_AspNetUsers_FollowerId",
                table: "userFollows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
