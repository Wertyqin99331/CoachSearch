using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachSearch.Migrations
{
    /// <inheritdoc />
    public partial class ChangeReviewsTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Customers_CustomerId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Trainers_TrainerId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_TrainingPrograms_TrainingProgramId",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "Reviews");

            migrationBuilder.RenameIndex(
                name: "IX_Review_TrainingProgramId",
                table: "Reviews",
                newName: "IX_Reviews_TrainingProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_TrainerId",
                table: "Reviews",
                newName: "IX_Reviews_TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_CustomerId",
                table: "Reviews",
                newName: "IX_Reviews_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Customers_CustomerId",
                table: "Reviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Trainers_TrainerId",
                table: "Reviews",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "TrainerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_TrainingPrograms_TrainingProgramId",
                table: "Reviews",
                column: "TrainingProgramId",
                principalTable: "TrainingPrograms",
                principalColumn: "TrainingProgramId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Customers_CustomerId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Trainers_TrainerId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_TrainingPrograms_TrainingProgramId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Review");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_TrainingProgramId",
                table: "Review",
                newName: "IX_Review_TrainingProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_TrainerId",
                table: "Review",
                newName: "IX_Review_TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_CustomerId",
                table: "Review",
                newName: "IX_Review_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Customers_CustomerId",
                table: "Review",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Trainers_TrainerId",
                table: "Review",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "TrainerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_TrainingPrograms_TrainingProgramId",
                table: "Review",
                column: "TrainingProgramId",
                principalTable: "TrainingPrograms",
                principalColumn: "TrainingProgramId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
