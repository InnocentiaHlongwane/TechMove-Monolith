using System.Text;
using System.Text.Json;
using TechMove.Core.Models;

namespace TechMove.Web.Services;

/// <summary>
/// HTTP client implementation for Contract API calls
/// </summary>
public class ContractApiClient : IContractApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ContractApiClient> _logger;

    public ContractApiClient(HttpClient httpClient, ILogger<ContractApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Contract>> GetContractsAsync(DateTime? startDate = null, DateTime? endDate = null, ContractStatus? status = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate:yyyy-MM-dd}");
            if (status.HasValue)
                queryParams.Add($"status={status}");

            var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"api/contracts{query}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Error fetching contracts: {response.StatusCode}");
                return new List<Contract>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<Contract>>(json, options) ?? new List<Contract>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GetContractsAsync");
            return new List<Contract>();
        }
    }

    public async Task<Contract> GetContractAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/contracts/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Contract {id} not found: {response.StatusCode}");
                return null!;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Contract>(json, options)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling GetContractAsync for id {id}");
            return null!;
        }
    }

    public async Task<Contract> CreateContractAsync(int clientId, DateTime startDate, DateTime endDate, string serviceLevel, IFormFile signedAgreement)
    {
        try
        {
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(clientId.ToString()), "clientId");
                form.Add(new StringContent(startDate.ToString("yyyy-MM-dd")), "startDate");
                form.Add(new StringContent(endDate.ToString("yyyy-MM-dd")), "endDate");
                form.Add(new StringContent(serviceLevel), "serviceLevel");

                if (signedAgreement != null && signedAgreement.Length > 0)
                {
                    var stream = signedAgreement.OpenReadStream();
                    form.Add(new StreamContent(stream), "signedAgreement", signedAgreement.FileName);
                }

                var response = await _httpClient.PostAsync("api/contracts", form);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Error creating contract: {response.StatusCode} - {error}");
                    throw new Exception($"Failed to create contract: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Contract>(json, options)!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CreateContractAsync");
            throw;
        }
    }

    public async Task<Contract> UpdateContractStatusAsync(int id, string status)
    {
        try
        {
            var body = new { status };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"api/contracts/{id}/status", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Error updating contract status: {response.StatusCode} - {error}");
                throw new Exception($"Failed to update contract status: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Contract>(responseJson, options)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling UpdateContractStatusAsync for id {id}");
            throw;
        }
    }

    public async Task<byte[]> DownloadContractAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/contracts/{id}/download");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Error downloading contract {id}: {response.StatusCode}");
                return Array.Empty<byte>();
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling DownloadContractAsync for id {id}");
            return Array.Empty<byte>();
        }
    }
}
