using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineJudge.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDifficultyAndExampleToProblems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "Problems",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Example",
                table: "Problems",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "Example",
                table: "Problems");
        }
    }
}
