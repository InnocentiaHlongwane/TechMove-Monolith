namespace TechMove.API.Dtos;

/// <summary>
/// Data Transfer Object for Contract API responses and requests
/// </summary>
public class ContractDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string? ClientName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ServiceLevel { get; set; } = string.Empty;
    public string SignedAgreementPath { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a new contract
/// </summary>
public class CreateContractDto
{
    public int ClientId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ServiceLevel { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating contract status
/// </summary>
public class UpdateContractStatusDto
{
    public string Status { get; set; } = string.Empty;
}
