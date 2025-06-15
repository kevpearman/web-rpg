using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerchantRPG.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameMaps",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    RewardRanges = table.Column<string>(type: "text", nullable: false),
                    UnlocksMapId = table.Column<string>(type: "character varying(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameMaps_GameMaps_UnlocksMapId",
                        column: x => x.UnlocksMapId,
                        principalTable: "GameMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Gold = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastActive = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adventurers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Strength = table.Column<int>(type: "integer", nullable: false),
                    Agility = table.Column<int>(type: "integer", nullable: false),
                    Intelligence = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    IsOnMission = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adventurers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adventurers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerResources_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdventurerId = table.Column<int>(type: "integer", nullable: false),
                    MapId = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Rewards = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Missions_Adventurers_AdventurerId",
                        column: x => x.AdventurerId,
                        principalTable: "Adventurers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GameMaps",
                columns: new[] { "Id", "Difficulty", "Duration", "Name", "RequiredLevel", "RewardRanges", "UnlocksMapId" },
                values: new object[,]
                {
                    { "dark_cave", "medium", 900, "Dark Cave", 5, "{\"gold\": {\"min\": 50, \"max\": 80}, \"materials\": [{\"type\": \"gems\", \"min\": 1, \"max\": 2}, {\"type\": \"rare_metals\", \"min\": 1, \"max\": 1}]}", null },
                    { "goblin_camp", "easy", 600, "Goblin Camp", 2, "{\"gold\": {\"min\": 20, \"max\": 40}, \"materials\": [{\"type\": \"iron_ore\", \"min\": 1, \"max\": 2}, {\"type\": \"leather\", \"min\": 1, \"max\": 2}]}", "dark_cave" },
                    { "forest_clearing", "easy", 300, "Forest Clearing", 1, "{\"gold\": {\"min\": 10, \"max\": 25}, \"materials\": [{\"type\": \"wood\", \"min\": 1, \"max\": 3}]}", "goblin_camp" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adventurers_PlayerId_Name",
                table: "Adventurers",
                columns: new[] { "PlayerId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_GameMaps_UnlocksMapId",
                table: "GameMaps",
                column: "UnlocksMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_AdventurerId_Status",
                table: "Missions",
                columns: new[] { "AdventurerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Missions_StartTime",
                table: "Missions",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerResources_PlayerId_ResourceType",
                table: "PlayerResources",
                columns: new[] { "PlayerId", "ResourceType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_Email",
                table: "Players",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_Username",
                table: "Players",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameMaps");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "PlayerResources");

            migrationBuilder.DropTable(
                name: "Adventurers");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
