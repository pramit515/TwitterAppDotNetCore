using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterAppAPI.Migrations
{
    public partial class final_TweeterAPI_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "UserTweet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "UserTweet",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
