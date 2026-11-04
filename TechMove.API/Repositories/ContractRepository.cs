using Microsoft.EntityFrameworkCore;
using TechMove.Core.Data;
using TechMove.Core.Models;
using TechMove.Core.Services;

namespace TechMove.API.Repositories;

/// <summary>
/// Contract repository implementation using Entity Framework
/// </summary>
public class ContractRepository(AppDbContext db) : IContractRepository
{
    public async Task<Contract?> GetByIdAsync(int id)
    {
        return await db.Contracts
            .Include(c => c.Client)
            .Include(c => c.ServiceRequests)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Contract>> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, ContractStatus? status = null)
    {
        var query = db.Contracts
            .Include(c => c.Client)
            .Include(c => c.ServiceRequests)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(c => c.StartDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(c => c.EndDate <= endDate.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query.OrderByDescending(c => c.StartDate).ToListAsync();
    }

    public async Task<Contract> CreateAsync(Contract contract)
    {
        db.Contracts.Add(contract);
        await db.SaveChangesAsync();
        return contract;
    }

    public async Task<Contract> UpdateAsync(Contract contract)
    {
        db.Contracts.Update(contract);
        await db.SaveChangesAsync();
        return contract;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var contract = await db.Contracts.FindAsync(id);
        if (contract == null)
            return false;

        db.Contracts.Remove(contract);
        await db.SaveChangesAsync();
        return true;
    }
}
