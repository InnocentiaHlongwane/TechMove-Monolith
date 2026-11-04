using TechMove.Core.Models;

namespace TechMove.Web.Services;

/// <summary>
/// Interface for Contract API client operations
/// </summary>
public interface IContractApiClient
{
    Task<List<Contract>> GetContractsAsync(DateTime? startDate = null, DateTime? endDate = null, ContractStatus? status = null);
    Task<Contract> GetContractAsync(int id);
    Task<Contract> CreateContractAsync(int clientId, DateTime startDate, DateTime endDate, string serviceLevel, IFormFile signedAgreement);
    Task<Contract> UpdateContractStatusAsync(int id, string status);
    Task<byte[]> DownloadContractAsync(int id);
}
