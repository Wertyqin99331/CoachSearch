using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachSearch.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewTitleToReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_TrainingPrograms_TrainingProgramId",
                table: "Reviews");

            migrationBuilder.AlterColumn<long>(
                name: "TrainingProgramId",
                table: "Reviews",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "ReviewTitle",
                table: "Reviews",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_TrainingPrograms_TrainingProgramId",
                table: "Reviews",
                column: "TrainingProgramId",
                principalTable: "TrainingPrograms",
                principalColumn: "TrainingProgramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_TrainingPrograms_TrainingProgramId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewTitle",
                table: "Reviews");

            migrationBuilder.AlterColumn<long>(
                name: "TrainingProgramId",
                table: "Reviews",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_TrainingPrograms_TrainingProgramId",
                table: "Reviews",
                column: "TrainingProgramId",
                principalTable: "TrainingPrograms",
                principalColumn: "TrainingProgramId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
