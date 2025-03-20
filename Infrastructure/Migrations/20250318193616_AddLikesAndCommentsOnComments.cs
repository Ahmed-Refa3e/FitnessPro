using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesAndCommentsOnComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_postComments_AspNetUsers_UserId",
                table: "postComments");

            migrationBuilder.DropForeignKey(
                name: "FK_postComments_Posts_PostId",
                table: "postComments");

            migrationBuilder.DropForeignKey(
                name: "FK_postLikes_AspNetUsers_UserId",
                table: "postLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_postLikes_Posts_PostId",
                table: "postLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_postLikes",
                table: "postLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_postComments",
                table: "postComments");

            migrationBuilder.RenameTable(
                name: "postLikes",
                newName: "likes");

            migrationBuilder.RenameTable(
                name: "postComments",
                newName: "comments");

            migrationBuilder.RenameIndex(
                name: "IX_postLikes_UserId",
                table: "likes",
                newName: "IX_likes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_postLikes_PostId",
                table: "likes",
                newName: "IX_likes_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_postComments_UserId",
                table: "comments",
                newName: "IX_comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_postComments_PostId",
                table: "comments",
                newName: "IX_comments_PostId");

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "likes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "likes",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "comments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "comments",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_likes",
                table: "likes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comments",
                table: "comments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_likes_CommentId",
                table: "likes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_CommentId",
                table: "comments",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_Posts_PostId",
                table: "comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_CommentId",
                table: "comments",
                column: "CommentId",
                principalTable: "comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_likes_AspNetUsers_UserId",
                table: "likes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_likes_Posts_PostId",
                table: "likes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_likes_comments_CommentId",
                table: "likes",
                column: "CommentId",
                principalTable: "comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_Posts_PostId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_CommentId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_AspNetUsers_UserId",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_Posts_PostId",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_comments_CommentId",
                table: "likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_likes",
                table: "likes");

            migrationBuilder.DropIndex(
                name: "IX_likes_CommentId",
                table: "likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comments",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "IX_comments_CommentId",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "comments");

            migrationBuilder.RenameTable(
                name: "likes",
                newName: "postLikes");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "postComments");

            migrationBuilder.RenameIndex(
                name: "IX_likes_UserId",
                table: "postLikes",
                newName: "IX_postLikes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_likes_PostId",
                table: "postLikes",
                newName: "IX_postLikes_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_UserId",
                table: "postComments",
                newName: "IX_postComments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_PostId",
                table: "postComments",
                newName: "IX_postComments_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_postLikes",
                table: "postLikes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_postComments",
                table: "postComments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_postComments_AspNetUsers_UserId",
                table: "postComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_postComments_Posts_PostId",
                table: "postComments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_postLikes_AspNetUsers_UserId",
                table: "postLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_postLikes_Posts_PostId",
                table: "postLikes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
