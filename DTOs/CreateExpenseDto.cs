namespace ExpenseTrackerAPI.DTOs
{
    public class CreateExpenseDto
    {
        public decimal Amount { get; set; }

        public required string Title { get; set; }

        public required string Type { get; set;}

        public int CategoryId { get; set; }
    }
}