using System.Text;
using System.Text.Json;
using TechMove.Core.Models;

namespace TechMove.Web.Services;

/// <summary>
/// HTTP client implementation for Client API calls
/// </summary>
public class ClientApiClient : IClientApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ClientApiClient> _logger;

    public ClientApiClient(HttpClient httpClient, ILogger<ClientApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Client>> GetClientsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/clients");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Error fetching clients: {response.StatusCode}");
                return new List<Client>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<Client>>(json, options) ?? new List<Client>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GetClientsAsync");
            return new List<Client>();
        }
    }

    public async Task<Client> GetClientAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/clients/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Client {id} not found: {response.StatusCode}");
                return null!;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Client>(json, options)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling GetClientAsync for id {id}");
            return null!;
        }
    }

    public async Task<Client> CreateClientAsync(string name, string contactDetails, string region)
    {
        try
        {
            var body = new { name, contactDetails, region };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/clients", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Error creating client: {response.StatusCode} - {error}");
                throw new Exception($"Failed to create client: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Client>(responseJson, options)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CreateClientAsync");
            throw;
        }
    }

    public async Task<Client> UpdateClientAsync(int id, string name, string contactDetails, string region)
    {
        try
        {
            var body = new { name, contactDetails, region };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/clients/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Error updating client: {response.StatusCode} - {error}");
                throw new Exception($"Failed to update client: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Client>(responseJson, options)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling UpdateClientAsync for id {id}");
            throw;
        }
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Error deleting client {id}: {response.StatusCode}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling DeleteClientAsync for id {id}");
            return false;
        }
    }
}
