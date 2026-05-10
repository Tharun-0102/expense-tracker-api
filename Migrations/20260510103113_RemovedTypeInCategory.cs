using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expense_Tracker_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTypeInCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Categorys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Categorys",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
