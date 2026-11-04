using System.Text.Json;

namespace TechMove.Core.Services;

/// <summary>
/// Service for fetching live USD to ZAR exchange rates.
/// </summary>
public class CurrencyService(HttpClient httpClient)
{
    private const string ExchangeRateUrl = "https://api.exchangerate-api.com/v4/latest/USD";

    /// <summary>
    /// Gets the current USD to ZAR exchange rate.
    /// </summary>
    /// <exception cref="HttpRequestException">Thrown if the API call fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the rate cannot be parsed.</exception>
    public async Task<decimal> GetUsdToZarRateAsync()
    {
        using var response = await httpClient.GetAsync(ExchangeRateUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("rates", out var ratesElement) ||
            !ratesElement.TryGetProperty("ZAR", out var zarElement))
        {
            throw new InvalidOperationException("Could not parse exchange rate from API response.");
        }

        if (!zarElement.TryGetDecimal(out var rate))
        {
            throw new InvalidOperationException("Exchange rate value is not a valid decimal.");
        }

        return rate;
    }
}
