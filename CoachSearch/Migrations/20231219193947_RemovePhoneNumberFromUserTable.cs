using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachSearch.Migrations
{
    /// <inheritdoc />
    public partial class RemovePhoneNumberFromUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true);
        }
    }
}
