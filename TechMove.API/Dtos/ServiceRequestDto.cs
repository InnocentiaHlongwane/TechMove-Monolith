namespace TechMove.API.Dtos;

/// <summary>
/// Data Transfer Object for ServiceRequest API responses
/// </summary>
public class ServiceRequestDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a new service request
/// </summary>
public class CreateServiceRequestDto
{
    public int ContractId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
