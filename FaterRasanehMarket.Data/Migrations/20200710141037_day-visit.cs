using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FaterRasanehMarket.Data.Migrations
{
    public partial class dayvisit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DayVisits",
                columns: table => new
                {
                    DateTime = table.Column<DateTime>(nullable: false),
                    VisitCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayVisits", x => x.DateTime);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayVisits");
        }
    }
}
