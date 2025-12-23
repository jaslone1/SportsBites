using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDayParty.Migrations
{
    /// <inheritdoc />
    public partial class AddUpvoteCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UpvoteCount",
                table: "FoodSuggestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "FoodSuggestions",
                keyColumn: "FoodSuggestionId",
                keyValue: 1,
                column: "UpvoteCount",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpvoteCount",
                table: "FoodSuggestions");
        }
    }
}
