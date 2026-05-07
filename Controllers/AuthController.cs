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
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User request)
    {
        var userExist = await _context.Users
        .AnyAsync(x => x.Email == request.Email);

        if (userExist)
            return BadRequest("User Already Registered.");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);
        User user = new User()
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,

        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(new {success = true});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser request)
    {
        var user = await _context.Users
        .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
            return BadRequest("User not Found.");

        bool isPasswordHased = BCrypt.Net.BCrypt.Verify(request.PasswordHash, user.PasswordHash);

        if (!isPasswordHased)
            return BadRequest("InValid Password");

        string tokenString = CreateToken(user);
        return Ok(new {token = tokenString});
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name , user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]));

        var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );
        return new JwtSecurityTokenHandler()
               .WriteToken(token);
    }


    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok(new {sucess = "Protected Route"});
    }


}
