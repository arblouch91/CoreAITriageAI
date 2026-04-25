using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreTriageAI.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndAIDraftedResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AIDraftedResponse",
                table: "Complains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Complains",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIDraftedResponse",
                table: "Complains");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Complains");
        }
    }
}
