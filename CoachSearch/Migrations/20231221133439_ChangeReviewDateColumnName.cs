using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachSearch.Migrations
{
    /// <inheritdoc />
    public partial class ChangeReviewDateColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ReviewDate",
                table: "Review",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewDate",
                table: "Review");
        }
    }
}
