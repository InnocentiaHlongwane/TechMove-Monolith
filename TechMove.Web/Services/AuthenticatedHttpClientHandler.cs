namespace TechMove.Web.Services;

/// <summary>
/// HTTP message handler that automatically attaches JWT bearer token to API requests
/// </summary>
public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthenticatedHttpClientHandler> _logger;

    public AuthenticatedHttpClientHandler(IServiceProvider serviceProvider, ILogger<AuthenticatedHttpClientHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // Resolve authentication service from the service provider
            using (var scope = _serviceProvider.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                var token = authService.GetToken();

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    _logger.LogDebug("JWT token attached to API request");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to attach authentication token to request");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
