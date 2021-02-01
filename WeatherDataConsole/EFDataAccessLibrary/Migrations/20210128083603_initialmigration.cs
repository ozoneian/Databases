using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EFDataAccessLibrary.Migrations
{
    public partial class initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherSensors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherSensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Humidity = table.Column<byte>(type: "tinyint", nullable: true),
                    Temperature = table.Column<float>(type: "real", nullable: true),
                    Collected_On = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeatherSensorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherDatas_WeatherSensors_WeatherSensorId",
                        column: x => x.WeatherSensorId,
                        principalTable: "WeatherSensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherDatas_WeatherSensorId",
                table: "WeatherDatas",
                column: "WeatherSensorId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherSensors_Location",
                table: "WeatherSensors",
                column: "Location",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherDatas");

            migrationBuilder.DropTable(
                name: "WeatherSensors");
        }
    }
}
