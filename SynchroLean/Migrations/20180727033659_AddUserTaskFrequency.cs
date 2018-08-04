using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class AddUserTaskFrequency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "frequency",
                table: "UserTasks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "frequency",
                table: "UserTasks");
        }
    }
}
