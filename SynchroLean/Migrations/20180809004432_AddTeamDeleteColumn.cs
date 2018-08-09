using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class AddTeamDeleteColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Todos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "Teams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Todos_TeamId",
                table: "Todos",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Teams_TeamId",
                table: "Todos",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Teams_TeamId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_TeamId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Teams");
        }
    }
}
