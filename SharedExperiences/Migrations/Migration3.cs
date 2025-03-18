using Microsoft.EntityFrameworkCore.Migrations;

namespace ExperienceService.Migrations
{
    public partial class Migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First convert decimal to int by truncating the cents part
            migrationBuilder.Sql(
                @"UPDATE Services SET Price = CAST(Price AS INT)");

            // Then change the column type to int
            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Services",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Change column type back to decimal
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // No need to convert data back as the int value will be preserved in the decimal
        }
    }
}