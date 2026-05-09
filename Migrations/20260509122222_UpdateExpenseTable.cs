using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expense_Tracker_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExpenseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Expenses",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Expenses",
                newName: "Date");
        }
    }
}
