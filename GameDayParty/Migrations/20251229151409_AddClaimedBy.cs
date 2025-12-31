using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDayParty.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimedByName",
                table: "FoodSuggestions",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "FoodSuggestions",
                keyColumn: "FoodSuggestionId",
                keyValue: 1,
                column: "ClaimedByName",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimedByName",
                table: "FoodSuggestions");
        }
    }
}
