using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pustok_BackEndProject.Migrations
{
    public partial class updatedProductsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Extax",
                table: "Products",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AddColumn<decimal>(
                name: "Count",
                table: "Products",
                type: "money",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Products");

            migrationBuilder.AlterColumn<decimal>(
                name: "Extax",
                table: "Products",
                type: "money",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
