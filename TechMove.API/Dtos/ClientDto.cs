namespace TechMove.API.Dtos;

/// <summary>
/// Data Transfer Object for Client API responses
/// </summary>
public class ClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactDetails { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a new client
/// </summary>
public class CreateClientDto
{
    public string Name { get; set; } = string.Empty;
    public string ContactDetails { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
