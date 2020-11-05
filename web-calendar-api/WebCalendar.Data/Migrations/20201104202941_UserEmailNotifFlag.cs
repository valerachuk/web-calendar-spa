using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCalendar.Data.Migrations
{
    public partial class UserEmailNotifFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceiveEmailNotifications",
                table: "Users",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiveEmailNotifications",
                table: "Users");
        }
    }
}
