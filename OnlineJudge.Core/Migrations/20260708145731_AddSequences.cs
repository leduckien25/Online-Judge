using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineJudge.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSequences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "ProblemSequence");

            migrationBuilder.CreateSequence<int>(
                name: "SubmissionSequence");

            migrationBuilder.CreateSequence<int>(
                name: "TestCaseSequence");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TestCases",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValueSql: "'TC' || lpad(nextval('\"TestCaseSequence\"')::text, 3, '0')",
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Submissions",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValueSql: "'SUB' || lpad(nextval('\"SubmissionSequence\"')::text, 3, '0')",
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Problems",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValueSql: "'PRO' || lpad(nextval('\"ProblemSequence\"')::text, 3, '0')",
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "ProblemSequence");

            migrationBuilder.DropSequence(
                name: "SubmissionSequence");

            migrationBuilder.DropSequence(
                name: "TestCaseSequence");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TestCases",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldDefaultValueSql: "'TC' || lpad(nextval('\"TestCaseSequence\"')::text, 3, '0')");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Submissions",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldDefaultValueSql: "'SUB' || lpad(nextval('\"SubmissionSequence\"')::text, 3, '0')");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Problems",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldDefaultValueSql: "'PRO' || lpad(nextval('\"ProblemSequence\"')::text, 3, '0')");
        }
    }
}
