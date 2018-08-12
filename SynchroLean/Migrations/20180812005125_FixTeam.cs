using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class FixTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Teams_OwnerEmail",
                table: "Teams",
                column: "OwnerEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_UserAccounts_OwnerEmail",
                table: "Teams",
                column: "OwnerEmail",
                principalTable: "UserAccounts",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_UserAccounts_OwnerEmail",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_OwnerEmail",
                table: "Teams");
        }
    }
}
