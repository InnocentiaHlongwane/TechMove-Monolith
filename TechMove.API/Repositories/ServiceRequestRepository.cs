using Microsoft.EntityFrameworkCore;
using TechMove.Core.Data;
using TechMove.Core.Models;
using TechMove.Core.Services;

namespace TechMove.API.Repositories;

/// <summary>
/// Service request repository implementation using Entity Framework
/// </summary>
public class ServiceRequestRepository(AppDbContext db) : IServiceRequestRepository
{
    public async Task<ServiceRequest?> GetByIdAsync(int id)
    {
        return await db.ServiceRequests
            .Include(sr => sr.Contract)
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public async Task<List<ServiceRequest>> GetByContractIdAsync(int contractId)
    {
        return await db.ServiceRequests
            .Where(sr => sr.ContractId == contractId)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ServiceRequest>> GetAllAsync()
    {
        return await db.ServiceRequests
            .Include(sr => sr.Contract)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync();
    }

    public async Task<ServiceRequest> CreateAsync(ServiceRequest request)
    {
        db.ServiceRequests.Add(request);
        await db.SaveChangesAsync();
        return request;
    }

    public async Task<ServiceRequest> UpdateAsync(ServiceRequest request)
    {
        db.ServiceRequests.Update(request);
        await db.SaveChangesAsync();
        return request;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var request = await db.ServiceRequests.FindAsync(id);
        if (request == null)
            return false;

        db.ServiceRequests.Remove(request);
        await db.SaveChangesAsync();
        return true;
    }
}
