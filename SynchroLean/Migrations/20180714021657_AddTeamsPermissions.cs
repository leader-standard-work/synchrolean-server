using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class AddTeamsPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamPermissions",
                columns: table => new
                {
                    SubjectTeamId = table.Column<int>(nullable: false),
                    ObjectTeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPermissions", x => new { x.SubjectTeamId, x.ObjectTeamId });
                    table.ForeignKey(
                        name: "FK_TeamPermissions_Teams_ObjectTeamId",
                        column: x => x.ObjectTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPermissions_Teams_SubjectTeamId",
                        column: x => x.SubjectTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamPermissions_ObjectTeamId",
                table: "TeamPermissions",
                column: "ObjectTeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamPermissions");
        }
    }
}
