using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class AddDeletedToTaskAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserAccounts");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "UserTasks",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "UserAccounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserAccounts");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "UserTasks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserAccounts",
                nullable: false,
                defaultValue: false);
        }
    }
}
