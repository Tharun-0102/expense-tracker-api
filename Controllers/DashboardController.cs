using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public DashboardController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [Authorize]
    [HttpGet("today-transaction")]
    public async Task<IActionResult> TodaysTransaction()
    {
        var userId =
            User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var today = DateTime.UtcNow.Date;

        var transactions =
            await _context.Expenses
                .Where(x =>
                    x.UserId == int.Parse(userId)
                    &&
                    x.CreatedAt.Date == today
                )
                .OrderByDescending(x =>
                    x.CreatedAt
                )
                .Select(x => new
                {
                    x.Id,
                    x.Amount,
                    x.Title,
                    x.Type,
                    x.CategoryId,
                    Date = x.CreatedAt
                    .ToLocalTime()
                    .ToString("yyyy-MM-dd"),
                    Time = x.CreatedAt
                    .ToLocalTime()
                    .ToString("hh:mm tt")
                })
                .ToListAsync();
        return Ok(transactions);
    }
}