using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TechMove.API.Controllers;

/// <summary>
/// API authentication endpoint for obtaining JWT tokens
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// This is a simple demo implementation - in production, validate against a user database
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request?.Password))
        {
            _logger.LogWarning("Login attempt with missing credentials");
            return BadRequest(new { message = "Username and password are required" });
        }

        // Demo validation - in production, check against user database with password hashing
        if (!ValidateUser(request.Username, request.Password))
        {
            _logger.LogWarning($"Failed login attempt for user: {request.Username}");
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var token = GenerateJwtToken(request.Username);
        _logger.LogInformation($"User {request.Username} authenticated successfully");

        return Ok(new { token, message = "Login successful" });
    }

    /// <summary>
    /// Demo user validation - in production, check against proper user database with password hashing
    /// </summary>
    private bool ValidateUser(string username, string password)
    {
        // Demo credentials - replace with actual user database validation in production
        var demoUsers = new Dictionary<string, string>
        {
            { "admin", "admin123" },
            { "user", "user123" },
            { "lebo", "password123" }
        };

        return demoUsers.ContainsKey(username) && demoUsers[username] == password;
    }

    /// <summary>
    /// Generate JWT token for authenticated user
    /// </summary>
    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "TechMove";
        var audience = jwtSettings["Audience"] ?? "TechMoveApp";
        var expirationMinutes = int.TryParse(jwtSettings["ExpirationMinutes"], out var minutes) ? minutes : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Name, username),
            new Claim("aud", audience)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
