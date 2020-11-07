using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCalendar.Data.Migrations
{
    public partial class Event_Notification_Job_Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotificationJobId",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationJobId",
                table: "Events");
        }
    }
}
