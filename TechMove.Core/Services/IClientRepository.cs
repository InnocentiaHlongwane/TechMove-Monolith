using TechMove.Core.Models;

namespace TechMove.Core.Services;

/// <summary>
/// Client repository interface for data access operations
/// </summary>
public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
    Task<List<Client>> GetAllAsync();
    Task<Client> CreateAsync(Client client);
    Task<Client> UpdateAsync(Client client);
    Task<bool> DeleteAsync(int id);
}
