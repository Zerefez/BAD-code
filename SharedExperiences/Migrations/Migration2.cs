using Microsoft.EntityFrameworkCore.Migrations;

namespace ExperienceService.Migrations
{
    public partial class Migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVR",
                table: "Providers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CVR",
                table: "Providers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}