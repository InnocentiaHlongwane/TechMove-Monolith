using TechMove.Core.Models;

namespace TechMove.Web.Services;

/// <summary>
/// Interface for Client API client operations
/// </summary>
public interface IClientApiClient
{
    Task<List<Client>> GetClientsAsync();
    Task<Client> GetClientAsync(int id);
    Task<Client> CreateClientAsync(string name, string contactDetails, string region);
    Task<Client> UpdateClientAsync(int id, string name, string contactDetails, string region);
    Task<bool> DeleteClientAsync(int id);
}
