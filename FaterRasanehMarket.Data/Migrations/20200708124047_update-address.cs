using Microsoft.EntityFrameworkCore.Migrations;

namespace FaterRasanehMarket.Data.Migrations
{
    public partial class updateaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingName",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Addresses");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefualt",
                table: "Addresses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Addresses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Addresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefualt",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "BuildingName",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
