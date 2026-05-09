namespace ExpenseTrackerAPI.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public required string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public int UserId { get; set; }
    }
}