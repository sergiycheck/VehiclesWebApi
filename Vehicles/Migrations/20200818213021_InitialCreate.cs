using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vehicles.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarOwners",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    SurName = table.Column<string>(nullable: true),
                    Patronymic = table.Column<string>(nullable: true),
                    CarOwnerPhone = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarOwners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniqueNumber = table.Column<string>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CarEngine = table.Column<float>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Transmision = table.Column<string>(nullable: true),
                    Drive = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManyToManyCarOwners",
                columns: table => new
                {
                    CarId = table.Column<int>(nullable: false),
                    CarOwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManyToManyCarOwners", x => new { x.CarOwnerId, x.CarId });
                    table.ForeignKey(
                        name: "FK_ManyToManyCarOwners_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManyToManyCarOwners_CarOwners_CarOwnerId",
                        column: x => x.CarOwnerId,
                        principalTable: "CarOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManyToManyCarOwners_CarId",
                table: "ManyToManyCarOwners",
                column: "CarId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManyToManyCarOwners");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "CarOwners");
        }
    }
}
