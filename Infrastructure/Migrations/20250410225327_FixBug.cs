using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnlineTrainings_AspNetUsers_CoachID",
                table: "OnlineTrainings");

            migrationBuilder.RenameColumn(
                name: "CoachID",
                table: "OnlineTrainings",
                newName: "CoachId");

            migrationBuilder.RenameIndex(
                name: "IX_OnlineTrainings_CoachID",
                table: "OnlineTrainings",
                newName: "IX_OnlineTrainings_CoachId");

            migrationBuilder.AlterColumn<string>(
                name: "CoachId",
                table: "OnlineTrainings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CoachID",
                table: "OnlineTrainings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineTrainings_CoachID",
                table: "OnlineTrainings",
                column: "CoachID");

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineTrainings_AspNetUsers_CoachID",
                table: "OnlineTrainings",
                column: "CoachID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineTrainings_AspNetUsers_CoachId",
                table: "OnlineTrainings",
                column: "CoachId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnlineTrainings_AspNetUsers_CoachID",
                table: "OnlineTrainings");

            migrationBuilder.DropForeignKey(
                name: "FK_OnlineTrainings_AspNetUsers_CoachId",
                table: "OnlineTrainings");

            migrationBuilder.DropIndex(
                name: "IX_OnlineTrainings_CoachID",
                table: "OnlineTrainings");

            migrationBuilder.DropColumn(
                name: "CoachID",
                table: "OnlineTrainings");

            migrationBuilder.RenameColumn(
                name: "CoachId",
                table: "OnlineTrainings",
                newName: "CoachID");

            migrationBuilder.RenameIndex(
                name: "IX_OnlineTrainings_CoachId",
                table: "OnlineTrainings",
                newName: "IX_OnlineTrainings_CoachID");

            migrationBuilder.AlterColumn<string>(
                name: "CoachID",
                table: "OnlineTrainings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineTrainings_AspNetUsers_CoachID",
                table: "OnlineTrainings",
                column: "CoachID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
