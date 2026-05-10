using System.Security.Claims;
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExpenseController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("create-expense")]
    public async Task<IActionResult> CreateExpense(
        [FromBody] CreateExpenseDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        var indianTime = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            indianTimeZone
        );

        Expense expense = new Expense()
        {
            Amount = request.Amount,
            Title = request.Title,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow,
            CategoryId = request.CategoryId,
            UserId = int.Parse(userId!)
        };

        _context.Expenses.Add(expense);

        await _context.SaveChangesAsync();

        return Ok(expense);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var expense = await _context.Expenses
            .Where(x => x.UserId == int.Parse(userId!))
            .ToListAsync();

        return Ok(expense);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var expense = await _context.Expenses
            .FirstOrDefaultAsync(x => x.Id == id
                                   && x.UserId == int.Parse(userId!));

        if (expense == null)
            return NotFound("Expenses not found.");

        _context.Expenses.Remove(expense);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Expense deleted successfully." });
    }


    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(int id, [FromBody] CreateExpenseDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var expense = await _context.Expenses
            .FirstOrDefaultAsync(x => x.Id == id
                                   && x.UserId == int.Parse(userId!));

        if (expense == null)
            return NotFound("Expense not found.");

        var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        var indianTime = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            indianTimeZone
        );

        expense.Amount = request.Amount;
        expense.Title = request.Title;
        expense.Type = request.Type;
        expense.CreatedAt = DateTime.UtcNow;
        expense.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync();

        return Ok(expense);
    }
}