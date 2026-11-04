using TechMove.Core.Models;

namespace TechMove.Core.Services;

/// <summary>
/// Service request repository interface for data access operations
/// </summary>
public interface IServiceRequestRepository
{
    Task<ServiceRequest?> GetByIdAsync(int id);
    Task<List<ServiceRequest>> GetByContractIdAsync(int contractId);
    Task<List<ServiceRequest>> GetAllAsync();
    Task<ServiceRequest> CreateAsync(ServiceRequest request);
    Task<ServiceRequest> UpdateAsync(ServiceRequest request);
    Task<bool> DeleteAsync(int id);
}
