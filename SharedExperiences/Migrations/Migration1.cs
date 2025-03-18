using Microsoft.EntityFrameworkCore.Migrations;

namespace ExperienceService.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TouristicOperatorPermit",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TouristicOperatorPermit",
                table: "Providers");
        }
    }
}