using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using TechMove.Web.Services;

namespace TechMove.Web.Controllers;

/// <summary>
/// Handle user authentication
/// </summary>
public class LoginController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthenticationService _authService;
    private readonly ILogger<LoginController> _logger;

    public LoginController(IHttpClientFactory httpClientFactory, IAuthenticationService authService, ILogger<LoginController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Display login form
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        if (_authService.IsAuthenticated())
            return RedirectToAction("Index", "Home");

        return View();
    }

    /// <summary>
    /// Handle login request
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Username and password are required.");
            return View("Index");
        }

        try
        {
            var loginRequest = new { username, password };
            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Call API login endpoint using named HTTP client
            var httpClient = _httpClientFactory.CreateClient("AuthClient");
            var response = await httpClient.PostAsync("api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Login failed with status {response.StatusCode}");
                ModelState.AddModelError("", "Invalid username or password.");
                return View("Index");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var loginResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson, options);

            if (loginResponse?.Token == null)
            {
                ModelState.AddModelError("", "Failed to obtain authentication token.");
                return View("Index");
            }

            // Store token in session
            await _authService.SetTokenAsync(loginResponse.Token);

            _logger.LogInformation($"User {username} logged in successfully");
            TempData["SuccessMessage"] = "Login successful!";

            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error during login");
            ModelState.AddModelError("", "Unable to connect to authentication service.");
            return View("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            ModelState.AddModelError("", "An unexpected error occurred during login.");
            return View("Index");
        }
    }

    /// <summary>
    /// Handle logout
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.ClearTokenAsync();
        _logger.LogInformation("User logged out");
        TempData["SuccessMessage"] = "You have been logged out.";
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Response model for login API endpoint
    /// </summary>
    private class AuthResponse
    {
        public string? Token { get; set; }
        public string? Message { get; set; }
    }
}
