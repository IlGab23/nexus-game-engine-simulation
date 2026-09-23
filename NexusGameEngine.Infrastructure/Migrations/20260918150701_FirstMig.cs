using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusGameEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemsCatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaxStackQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionPayload = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsCatalog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AdminRole = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_SystemRoles_SystemRoleId",
                        column: x => x.SystemRoleId,
                        principalTable: "SystemRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Money = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsAlive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SpecialSkill_BonusPercentage = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: true),
                    SpecialSkill_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SpecialSkill_TargetStat = table.Column<int>(type: "int", nullable: true),
                    Constitution_Experience = table.Column<int>(type: "int", nullable: false),
                    Constitution_Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Constitution_Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Dexterity_Experience = table.Column<int>(type: "int", nullable: false),
                    Dexterity_Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Dexterity_Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Intelligence_Experience = table.Column<int>(type: "int", nullable: false),
                    Intelligence_Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Intelligence_Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MainLevel_Experience = table.Column<int>(type: "int", nullable: false),
                    MainLevel_Level = table.Column<byte>(type: "tinyint", nullable: false),
                    MainLevel_Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Health = table.Column<short>(type: "smallint", nullable: false),
                    MaxHealth = table.Column<short>(type: "smallint", nullable: false),
                    Stamina = table.Column<short>(type: "smallint", nullable: false),
                    PlayerStamina_LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MaxStamina = table.Column<short>(type: "smallint", nullable: false),
                    StaminaRegenRate = table.Column<short>(type: "smallint", nullable: false),
                    Strength_Experience = table.Column<int>(type: "int", nullable: false),
                    Strength_Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Strength_Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cooldowns = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Expiry = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokeAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReplacedByTokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventorySlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    StatePayload = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventorySlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventorySlots_ItemsCatalog_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemsCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventorySlots_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventorySlots_ItemId",
                table: "InventorySlots",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySlots_PlayerId",
                table: "InventorySlots",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemRoles_Name",
                table: "SystemRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SystemRoleId",
                table: "Users",
                column: "SystemRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventorySlots");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ItemsCatalog");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SystemRoles");
        }
    }
}
