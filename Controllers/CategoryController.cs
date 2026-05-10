using System.Security.Claims;
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("create-category")]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var catExist = await _context.Categorys
            .AnyAsync(x => x.Name == request.Name
                        && x.UserId == int.Parse(userId!));

        if (catExist)
            return BadRequest("Category already exists.");

        Category category = new Category()
        {
            Name = request.Name,
            UserId = int.Parse(userId!)
        };

        _context.Categorys.Add(category);

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var categories = await _context.Categorys
            .Where(x => x.UserId == int.Parse(userId!))
            .ToListAsync();

        return Ok(categories);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var category = await _context.Categorys
            .FirstOrDefaultAsync(x => x.Id == id
                                   && x.UserId == int.Parse(userId!));

        if (category == null)
            return NotFound("Category not found.");

        _context.Categorys.Remove(category);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Category deleted successfully." });
    }
}