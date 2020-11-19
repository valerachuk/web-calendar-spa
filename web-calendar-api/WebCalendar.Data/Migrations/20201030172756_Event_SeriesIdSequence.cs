using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCalendar.Data.Migrations
{
    public partial class Event_SeriesIdSequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "SeriesId_seq");

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                table: "Events",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR SeriesId_seq",
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "SeriesId_seq");

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValueSql: "NEXT VALUE FOR shared.SeriesId_seq");
        }
    }
}
