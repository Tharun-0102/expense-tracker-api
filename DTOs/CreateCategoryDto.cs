namespace ExpenseTrackerAPI.DTOs
{
    public class CreateCategoryDto
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
    }
}