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
    [HttpGet("todays-transaction/{type}")]
    public async Task<IActionResult> TodaysTransactionFilter(
    string type
)
    {
        var userId =
            User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var today =
            DateTime.UtcNow.Date;

        var query =
            _context.Expenses
                .Where(x =>
                    x.UserId ==
                        int.Parse(userId)
                    &&
                    x.CreatedAt.Date ==
                        today
                );

        // Filter
        if (type.ToLower() != "both")
        {
            query =
                query.Where(x =>
                    x.Type.Trim().ToLower() ==
                    type.Trim().ToLower()
                );
        }

        var transactions =
            await query

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

    [Authorize]
    [HttpGet("weekly-bar-chart")]
    public async Task<IActionResult> WeeklyBarChart()
    {
        var userId = User
            .FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        // Current Date
        var today = DateTime.UtcNow.Date;

        // Start Of Week (Monday)
        var startOfWeek =
            today.AddDays(
                -(int)today.DayOfWeek + 1
            );

        // If Sunday
        if (today.DayOfWeek == DayOfWeek.Sunday)
        {
            startOfWeek =
                today.AddDays(-6);
        }

        // End Of Week
        var endOfWeek =
            startOfWeek.AddDays(6);

        // Fetch Weekly Transactions
        var weeklyTransactions =
            await _context.Expenses

                .Where(x =>
                    x.UserId ==
                        int.Parse(userId)
                    &&
                    x.CreatedAt.Date >=
                        startOfWeek
                    &&
                    x.CreatedAt.Date <=
                        endOfWeek
                )

                .ToListAsync();

        // Days List
        var days = new[]
        {
        "Mon",
        "Tue",
        "Wed",
        "Thu",
        "Fri",
        "Sat",
        "Sun"
    };

        var result = new List<object>();

        // Generate Weekly Summary
        for (int i = 0; i < 7; i++)
        {
            var currentDay =
                startOfWeek.AddDays(i);

            var dayTransactions =
                weeklyTransactions
                    .Where(x =>
                        x.CreatedAt.Date ==
                        currentDay.Date
                    );

            var income =
                dayTransactions
                    .Where(x =>
                        x.Type.ToLower() ==
                        "income"
                    )
                    .Sum(x => x.Amount);

            var expense =
                dayTransactions
                    .Where(x =>
                        x.Type.ToLower() ==
                        "expense"
                    )
                    .Sum(x => x.Amount);

            result.Add(new
            {
                Day = days[i],

                Income = income,

                Expense = expense
            });
        }

        return Ok(result);
    }
}