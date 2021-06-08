using Microsoft.EntityFrameworkCore.Migrations;

namespace vehicles.Migrations
{
    public partial class AddProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarUniqueNumber",
                table: "Penalties",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarUniqueNumber",
                table: "Penalties");
        }
    }
}
