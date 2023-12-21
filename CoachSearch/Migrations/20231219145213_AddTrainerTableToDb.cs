using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachSearch.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainer_AspNetUsers_UserInfoId",
                table: "Trainer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer");

            migrationBuilder.RenameTable(
                name: "Trainer",
                newName: "Trainers");

            migrationBuilder.RenameIndex(
                name: "IX_Trainer_UserInfoId",
                table: "Trainers",
                newName: "IX_Trainers_UserInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainers",
                table: "Trainers",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_AspNetUsers_UserInfoId",
                table: "Trainers",
                column: "UserInfoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_AspNetUsers_UserInfoId",
                table: "Trainers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainers",
                table: "Trainers");

            migrationBuilder.RenameTable(
                name: "Trainers",
                newName: "Trainer");

            migrationBuilder.RenameIndex(
                name: "IX_Trainers_UserInfoId",
                table: "Trainer",
                newName: "IX_Trainer_UserInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainer_AspNetUsers_UserInfoId",
                table: "Trainer",
                column: "UserInfoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
