using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editGymPostRel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Gyms_GymId",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Gyms_GymId",
                table: "Posts",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "GymID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Gyms_GymId",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Gyms_GymId",
                table: "Posts",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "GymID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
