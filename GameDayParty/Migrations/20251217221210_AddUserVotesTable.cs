using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GameDayParty.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVotesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserVotes",
                columns: table => new
                {
                    UserVoteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FoodSuggestionId = table.Column<int>(type: "integer", nullable: false),
                    VoterName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVotes", x => x.UserVoteId);
                    table.ForeignKey(
                        name: "FK_UserVotes_FoodSuggestions_FoodSuggestionId",
                        column: x => x.FoodSuggestionId,
                        principalTable: "FoodSuggestions",
                        principalColumn: "FoodSuggestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserVotes_FoodSuggestionId",
                table: "UserVotes",
                column: "FoodSuggestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVotes");
        }
    }
}
