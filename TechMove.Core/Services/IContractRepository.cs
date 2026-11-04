using TechMove.Core.Models;

namespace TechMove.Core.Services;

/// <summary>
/// Contract repository interface for data access operations
/// </summary>
public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(int id);
    Task<List<Contract>> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, ContractStatus? status = null);
    Task<Contract> CreateAsync(Contract contract);
    Task<Contract> UpdateAsync(Contract contract);
    Task<bool> DeleteAsync(int id);
}
