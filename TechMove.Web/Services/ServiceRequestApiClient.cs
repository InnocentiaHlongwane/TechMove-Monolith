using System.Text;
using System.Text.Json;
using TechMove.Core.Models;

namespace TechMove.Web.Services;

/// <summary>
/// HTTP client implementation for Service Request API calls
/// </summary>
public class ServiceRequestApiClient : IServiceRequestApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceRequestApiClient> _logger;

    public ServiceRequestApiClient(HttpClient httpClient, ILogger<ServiceRequestApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/servicerequests");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Error fetching service requests: {response.StatusCode}");
                return new List<ServiceRequest>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<ServiceRequest>>(json, options) ?? new List<ServiceRequest>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GetServiceRequestsAsync");
            return new List<ServiceRequest>();
        }
    }

    public async Task<List<ServiceRequest>> GetServiceRequestsByContractAsync(int contractId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/servicerequests/contract/{contractId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Error fetching service requests for contract {contractId}: {response.StatusCode}");
                return new List<ServiceRequest>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<ServiceRequest>>(json, options) ?? new List<ServiceRequest>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling GetServiceRequestsByContractAsync for contract {contractId}");
            return new List<ServiceRequest>();
        }
    }

    public async Task<ServiceRequest> GetServiceRequestAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/servicerequests/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Service request {id} not found: {response.StatusCode}");
                return null!;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<ServiceRequest>(json, options)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling GetServiceRequestAsync for id {id}");
            return null!;
        }
    }

    public async Task<ServiceRequest> CreateServiceRequestAsync(int contractId, string description, decimal cost, string? currency = null)
    {
        try
        {
            var body = new { contractId, description, cost, currency };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/servicerequests", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Error creating service request: {response.StatusCode} - {error}");
                throw new Exception($"Failed to create service request: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<ServiceRequest>(responseJson, options)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CreateServiceRequestAsync");
            throw;
        }
    }

    public async Task<ServiceRequest> UpdateServiceRequestAsync(int id, string description, decimal cost, string? currency = null)
    {
        try
        {
            var body = new { description, cost, currency };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/servicerequests/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Error updating service request: {response.StatusCode} - {error}");
                throw new Exception($"Failed to update service request: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<ServiceRequest>(responseJson, options)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling UpdateServiceRequestAsync for id {id}");
            throw;
        }
    }

    public async Task<bool> DeleteServiceRequestAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/servicerequests/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Error deleting service request {id}: {response.StatusCode}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling DeleteServiceRequestAsync for id {id}");
            return false;
        }
    }
}
