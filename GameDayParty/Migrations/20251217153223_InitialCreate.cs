using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GameDayParty.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GameDetails = table.Column<string>(type: "text", nullable: false),
                    HostUserId = table.Column<int>(type: "integer", nullable: false),
                    HostName = table.Column<string>(type: "text", nullable: false),
                    IsFinalized = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "FoodSuggestions",
                columns: table => new
                {
                    FoodSuggestionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    FoodName = table.Column<string>(type: "text", nullable: false),
                    SuggestedByName = table.Column<string>(type: "text", nullable: false),
                    FoodCategory = table.Column<string>(type: "text", nullable: false),
                    IsVegetarian = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodSuggestions", x => x.FoodSuggestionId);
                    table.ForeignKey(
                        name: "FK_FoodSuggestions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "EventDate", "EventName", "GameDetails", "HostName", "HostUserId", "IsFinalized" },
                values: new object[] { 1, new DateTime(2025, 12, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Sunday Night Football Fiesta", "Packers vs. Vikings (NFC North)", "Jared Slone", 0, false });

            migrationBuilder.InsertData(
                table: "FoodSuggestions",
                columns: new[] { "FoodSuggestionId", "EventId", "FoodCategory", "FoodName", "IsVegetarian", "SuggestedByName" },
                values: new object[] { 1, 1, "", "Buffalo Wings", false, "Jared" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodSuggestions_EventId",
                table: "FoodSuggestions",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodSuggestions");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
