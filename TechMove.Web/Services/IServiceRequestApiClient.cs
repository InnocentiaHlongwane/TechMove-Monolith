using TechMove.Core.Models;

namespace TechMove.Web.Services;

/// <summary>
/// Interface for Service Request API client operations
/// </summary>
public interface IServiceRequestApiClient
{
    Task<List<ServiceRequest>> GetServiceRequestsAsync();
    Task<List<ServiceRequest>> GetServiceRequestsByContractAsync(int contractId);
    Task<ServiceRequest> GetServiceRequestAsync(int id);
    Task<ServiceRequest> CreateServiceRequestAsync(int contractId, string description, decimal cost, string? currency = null);
    Task<ServiceRequest> UpdateServiceRequestAsync(int id, string description, decimal cost, string? currency = null);
    Task<bool> DeleteServiceRequestAsync(int id);
}
