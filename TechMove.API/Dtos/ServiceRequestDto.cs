namespace TechMove.API.Dtos;

/// <summary>
/// Data Transfer Object for ServiceRequest API responses
/// </summary>
public class ServiceRequestDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Currency { get; set; } = "ZAR";
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new service request
/// </summary>
public class CreateServiceRequestDto
{
    public int ContractId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string? Currency { get; set; }
}
