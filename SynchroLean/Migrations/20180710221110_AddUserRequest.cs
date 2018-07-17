using Microsoft.EntityFrameworkCore.Migrations;

namespace SynchroLean.Migrations
{
    public partial class AddUserRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddUserRequests",
                columns: table => new
                {
                    AddUserRequestId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DestinationTeamId = table.Column<int>(nullable: false),
                    InviteeOwnerId = table.Column<int>(nullable: false),
                    InviterOwnerId = table.Column<int>(nullable: true),
                    IsAuthorized = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddUserRequests", x => x.AddUserRequestId);
                    table.ForeignKey(
                        name: "FK_AddUserRequests_Teams_DestinationTeamId",
                        column: x => x.DestinationTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddUserRequests_UserAccounts_InviteeOwnerId",
                        column: x => x.InviteeOwnerId,
                        principalTable: "UserAccounts",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddUserRequests_UserAccounts_InviterOwnerId",
                        column: x => x.InviterOwnerId,
                        principalTable: "UserAccounts",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_DestinationTeamId",
                table: "AddUserRequests",
                column: "DestinationTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_InviteeOwnerId",
                table: "AddUserRequests",
                column: "InviteeOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AddUserRequests_InviterOwnerId",
                table: "AddUserRequests",
                column: "InviterOwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddUserRequests");
        }
    }
}
