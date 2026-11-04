using Microsoft.EntityFrameworkCore;
using TechMove.Core.Data;
using TechMove.Core.Models;
using TechMove.Core.Services;

namespace TechMove.API.Repositories;

/// <summary>
/// Client repository implementation using Entity Framework
/// </summary>
public class ClientRepository(AppDbContext db) : IClientRepository
{
    public async Task<Client?> GetByIdAsync(int id)
    {
        return await db.Clients
            .Include(c => c.Contracts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Client>> GetAllAsync()
    {
        return await db.Clients
            .Include(c => c.Contracts)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Client> CreateAsync(Client client)
    {
        db.Clients.Add(client);
        await db.SaveChangesAsync();
        return client;
    }

    public async Task<Client> UpdateAsync(Client client)
    {
        db.Clients.Update(client);
        await db.SaveChangesAsync();
        return client;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var client = await db.Clients.FindAsync(id);
        if (client == null)
            return false;

        db.Clients.Remove(client);
        await db.SaveChangesAsync();
        return true;
    }
}
