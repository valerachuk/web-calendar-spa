using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCalendar.Data.Migrations
{
    public partial class addFileEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventFile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    UniqueName = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    UploadDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventFile", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_FileId",
                table: "Events",
                column: "FileId",
                unique: true,
                filter: "[FileId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventFile_FileId",
                table: "Events",
                column: "FileId",
                principalTable: "EventFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventFile_FileId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventFile");

            migrationBuilder.DropIndex(
                name: "IX_Events_FileId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Events");
        }
    }
}
