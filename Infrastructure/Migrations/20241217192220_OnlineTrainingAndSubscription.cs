using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OnlineTrainingAndSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnlineTrainingSubscriptions_OnlineTrainings_TrainingID",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropTable(
                name: "OnlineTraining");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OnlineTrainingSubscriptions",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_OnlineTrainingSubscriptions_TrainingID",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionID",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OnlineTrainings");

            migrationBuilder.RenameColumn(
                name: "TrainingID",
                table: "OnlineTrainingSubscriptions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TrainingID",
                table: "OnlineTrainings",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "TraineeID",
                table: "OnlineTrainingSubscriptions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OnlineTrainingSubscriptions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "OnlineTrainingSubscriptions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OnlineTrainingId",
                table: "OnlineTrainingSubscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrainingType",
                table: "OnlineTrainings",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "OnlineTrainings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OnlineTrainings",
                type: "decimal(6,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<byte>(
                name: "NoOfSessionsPerWeek",
                table: "OnlineTrainings",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "DurationOfSession",
                table: "OnlineTrainings",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "OnlineTrainings",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CoachID",
                table: "OnlineTrainings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<byte>(
                name: "DurationUnit",
                table: "OnlineTrainings",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OfferEnded",
                table: "OnlineTrainings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OfferPrice",
                table: "OnlineTrainings",
                type: "decimal(6,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "OnlineTrainings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionClosed",
                table: "OnlineTrainings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OnlineTrainingSubscriptions",
                table: "OnlineTrainingSubscriptions",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach1",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "1bd92dbb-5bee-4f07-ad16-384f6ce873a0", new DateTime(2024, 12, 17, 14, 22, 18, 174, DateTimeKind.Local).AddTicks(205), "6c7d38ca-ec42-4c12-87fd-08442ae2d030" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "coach2",
                columns: new[] { "ConcurrencyStamp", "JoinedDate", "SecurityStamp" },
                values: new object[] { "78b91a77-4995-4b54-9b12-40c5339ce155", new DateTime(2024, 12, 17, 14, 22, 18, 174, DateTimeKind.Local).AddTicks(326), "4fa46862-8f2b-404d-8669-f5eec21c86ef" });

            migrationBuilder.CreateIndex(
                name: "IX_OnlineTrainingSubscriptions_OnlineTrainingId",
                table: "OnlineTrainingSubscriptions",
                column: "OnlineTrainingId");

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineTrainings_AspNetUsers_CoachID",
                table: "OnlineTrainings",
                column: "CoachID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineTrainingSubscriptions_OnlineTrainings_OnlineTrainingId",
                table: "OnlineTrainingSubscriptions",
                column: "OnlineTrainingId",
                principalTable: "OnlineTrainings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnlineTrainings_AspNetUsers_CoachID",
                table: "OnlineTrainings");

            migrationBuilder.DropForeignKey(
                name: "FK_OnlineTrainingSubscriptions_OnlineTrainings_OnlineTrainingId",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OnlineTrainingSubscriptions",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_OnlineTrainingSubscriptions_OnlineTrainingId",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropColumn(
                name: "OnlineTrainingId",
                table: "OnlineTrainingSubscriptions");

            migrationBuilder.DropColumn(
                name: "DurationUnit",
                table: "OnlineTrainings");

            migrationBuilder.DropColumn(
                name: "OfferEnded",
                table: "OnlineTrainings");

            migrationBuilder.DropColumn(
                name: "OfferPrice",
                table: "OnlineTrainings");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "OnlineTrainings");

            migrationBuilder.DropColumn(
                name: "SubscriptionClosed",
                table: "OnlineTrainings");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OnlineTrainingSubscriptions",
                newName: "TrainingID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OnlineTrainings",
                newName: "TrainingID");

            migrationBuilder.AlterColumn<string>(
                name: "TraineeID",
                table: "OnlineTrainingSubscriptions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrainingID",
                table: "OnlineTrainingSubscriptions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionID",
                table: "OnlineTrainingSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OnlineTrainingSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TrainingType",
                table: "OnlineTrainings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "OnlineTrainings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OnlineTrainings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)");

            migrationBuilder.AlterColumn<int>(
                name: "NoOfSessionsPerWeek",
                table: "OnlineTrainings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationOfSession",
                table: "OnlineTrainings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "OnlineTrainings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoachID",
                table: "OnlineTrainings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OnlineTrainings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OnlineTrainingSubscriptions",
                table: "OnlineTrainingSubscriptions",
                column: "SubscriptionID");

            migrationBuilder.CreateTable(
                name: "OnlineTraining",
                columns: table => new
                {
                    CoachID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_OnlineTraining_AspNetUsers_CoachID",
                        column: x => x.CoachID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_OnlineTrainingSubscriptions_TrainingID",
                table: "OnlineTrainingSubscriptions",
                column: "TrainingID");

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineTrainingSubscriptions_OnlineTrainings_TrainingID",
                table: "OnlineTrainingSubscriptions",
                column: "TrainingID",
                principalTable: "OnlineTrainings",
                principalColumn: "TrainingID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
