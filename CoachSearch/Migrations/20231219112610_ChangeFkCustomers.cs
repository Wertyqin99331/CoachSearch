using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachSearch.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFkCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserInfoId",
                table: "Customers",
                column: "UserInfoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_UserInfoId",
                table: "Customers",
                column: "UserInfoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_UserInfoId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserInfoId",
                table: "Customers");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
