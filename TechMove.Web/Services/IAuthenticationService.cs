namespace TechMove.Web.Services;

/// <summary>
/// Interface for managing authentication tokens
/// </summary>
public interface IAuthenticationService
{
    string? GetToken();
    Task SetTokenAsync(string token);
    Task ClearTokenAsync();
    bool IsAuthenticated();
}
