namespace TechMove.Web.Services;

/// <summary>
/// Simple session-based authentication service for storing JWT tokens
/// In production, consider using secure HTTP-only cookies or similar mechanisms
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private const string TokenKey = "auth_token";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor, ILogger<AuthenticationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Retrieve stored authentication token from session
    /// </summary>
    public string? GetToken()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        return session?.GetString(TokenKey);
    }

    /// <summary>
    /// Store authentication token in session
    /// </summary>
    public Task SetTokenAsync(string token)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session != null)
        {
            session.SetString(TokenKey, token);
            _logger.LogInformation("Token stored in session");
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Clear stored authentication token
    /// </summary>
    public Task ClearTokenAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session != null)
        {
            session.Remove(TokenKey);
            _logger.LogInformation("Token cleared from session");
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Check if user is authenticated (has a valid token)
    /// </summary>
    public bool IsAuthenticated()
    {
        var token = GetToken();
        return !string.IsNullOrEmpty(token);
    }
}
