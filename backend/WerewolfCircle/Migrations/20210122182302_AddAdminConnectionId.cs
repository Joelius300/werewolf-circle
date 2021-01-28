using Microsoft.EntityFrameworkCore.Migrations;

namespace WerewolfCircle.Migrations
{
    public partial class AddAdminConnectionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminConnectionId",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminConnectionId",
                table: "Games");
        }
    }
}
