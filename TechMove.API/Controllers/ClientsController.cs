using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMove.API.Dtos;
using TechMove.Core.Models;
using TechMove.Core.Services;

namespace TechMove.API.Controllers;

/// <summary>
/// API Controller for managing clients
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientRepository clientRepository) : ControllerBase
{
    /// <summary>
    /// Get all clients
    /// </summary>
    /// <returns>List of clients</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ClientDto>>> GetClients()
    {
        var clients = await clientRepository.GetAllAsync();
        var dtos = clients.Select(c => new ClientDto
        {
            Id = c.Id,
            Name = c.Name,
            ContactDetails = c.ContactDetails,
            Region = c.Region
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Get a specific client by ID
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <returns>Client details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var client = await clientRepository.GetByIdAsync(id);
        if (client == null)
            return NotFound();

        var dto = new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            ContactDetails = client.ContactDetails,
            Region = client.Region
        };

        return Ok(dto);
    }

    /// <summary>
    /// Create a new client
    /// </summary>
    /// <param name="dto">Client details</param>
    /// <returns>Created client</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto dto)
    {
        var client = new Client
        {
            Name = dto.Name,
            ContactDetails = dto.ContactDetails,
            Region = dto.Region
        };

        var created = await clientRepository.CreateAsync(client);

        var responseDto = new ClientDto
        {
            Id = created.Id,
            Name = created.Name,
            ContactDetails = created.ContactDetails,
            Region = created.Region
        };

        return CreatedAtAction(nameof(GetClient), new { id = created.Id }, responseDto);
    }

    /// <summary>
    /// Update a client
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <param name="dto">Updated client details</param>
    /// <returns>Updated client</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientDto>> UpdateClient(int id, [FromBody] CreateClientDto dto)
    {
        var client = await clientRepository.GetByIdAsync(id);
        if (client == null)
            return NotFound();

        client.Name = dto.Name;
        client.ContactDetails = dto.ContactDetails;
        client.Region = dto.Region;

        var updated = await clientRepository.UpdateAsync(client);

        var responseDto = new ClientDto
        {
            Id = updated.Id,
            Name = updated.Name,
            ContactDetails = updated.ContactDetails,
            Region = updated.Region
        };

        return Ok(responseDto);
    }

    /// <summary>
    /// Delete a client
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <returns>Success indicator</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var result = await clientRepository.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
