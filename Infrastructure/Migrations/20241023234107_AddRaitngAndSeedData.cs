using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRaitngAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "GymRating",
                columns: table => new
                {
                    GymRatingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingValue = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraineeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GymID = table.Column<int>(type: "int", nullable: false),
                    RatingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymRating", x => x.GymRatingID);
                    table.ForeignKey(
                        name: "FK_GymRating_AspNetUsers_TraineeID",
                        column: x => x.TraineeID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GymRating_Gyms_GymID",
                        column: x => x.GymID,
                        principalTable: "Gyms",
                        principalColumn: "GymID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AvailableForOnlineTraining", "Bio", "ConcurrencyStamp", "DateOfBirth", "Discriminator", "Email", "EmailConfirmed", "FirstName", "Gender", "JoinedDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "coach1", 0, true, null, "094621c4-1848-49a4-8a66-4195793c49c1", new DateTime(1985, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Coach", "johndoe@example.com", false, "John", "Male", new DateTime(2024, 10, 24, 2, 41, 7, 347, DateTimeKind.Local).AddTicks(999), "Doe", false, null, "JOHNDOE@EXAMPLE.COM", "JOHNDOE", null, "0123456789", false, null, "149fa698-1976-4742-8518-562d4b9578d8", false, "johndoe" },
                    { "coach2", 0, false, null, "5bb60ec0-fe4f-4d77-b3ac-ff5632479c82", new DateTime(1990, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Coach", "janesmith@example.com", false, "Jane", "Female", new DateTime(2024, 10, 24, 2, 41, 7, 347, DateTimeKind.Local).AddTicks(1145), "Smith", false, null, "JANESMITH@EXAMPLE.COM", "JANESMITH", null, "0987654321", false, null, "6c865888-3005-4854-8349-c637fa023349", false, "janesmith" }
                });

            migrationBuilder.InsertData(
                table: "Gyms",
                columns: new[] { "GymID", "Address", "City", "CoachID", "Country", "Description", "FortnightlyPrice", "GymName", "MonthlyPrice", "PhoneNumber", "PictureUrl", "SessionPrice", "YearlyPrice" },
                values: new object[,]
                {
                    { 1, "123 Main St", "Cairo", "coach1", "Egypt", "A top-tier gym with all the modern equipment you need.", 30, "Downtown Fitness", 50, "0123456789", null, 15, 500 },
                    { 2, "456 Sunset Blvd", "Alexandria", "coach2", "Egypt", "A wellness center focused on body and mind fitness.", 25, "Sunset Wellness", 40, "0987654321", null, 12, 450 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GymRating_GymID",
                table: "GymRating",
                column: "GymID");

            migrationBuilder.CreateIndex(
                name: "IX_GymRating_TraineeID",
                table: "GymRating",
                column: "TraineeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymRating");

            migrationBuilder.DeleteData(
                table: "Gyms",
                keyColumn: "GymID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Gyms",
                keyColumn: "GymID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");
        }
    }
}
