using Microsoft.EntityFrameworkCore.Migrations;

namespace FaterRasanehMarket.Data.Migrations
{
    public partial class fixvisit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Visits",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "LimitCount",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Visits",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CountLimit",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visits",
                table: "Visits",
                columns: new[] { "Id", "ProductId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ProductId",
                table: "Visits",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Visits",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_ProductId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "CountLimit",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "LimitCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visits",
                table: "Visits",
                columns: new[] { "ProductId", "UserId" });
        }
    }
}
