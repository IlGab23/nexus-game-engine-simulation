using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusGameEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MapZoneMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "BaseMovementSpeed",
                table: "Players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastMove",
                table: "Players",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "MapZoneId",
                table: "Players",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Pos_X",
                table: "Players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Pos_Y",
                table: "Players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Pos_Z",
                table: "Players",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "MapZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaxPlayers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapZones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_MapZoneId",
                table: "Players",
                column: "MapZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_MapZones_MapZoneId",
                table: "Players",
                column: "MapZoneId",
                principalTable: "MapZones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_MapZones_MapZoneId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "MapZones");

            migrationBuilder.DropIndex(
                name: "IX_Players_MapZoneId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BaseMovementSpeed",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastMove",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "MapZoneId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Pos_X",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Pos_Y",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Pos_Z",
                table: "Players");
        }
    }
}
