using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusGameEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MonsterTemplateMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonsterTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BaseHealth = table.Column<int>(type: "int", nullable: false),
                    BaseDamage = table.Column<int>(type: "int", nullable: false),
                    AttackRange = table.Column<float>(type: "real", nullable: false),
                    IsBoss = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LootTablePayload = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterTemplates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonsterTemplates");
        }
    }
}
