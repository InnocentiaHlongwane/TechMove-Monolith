using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.API.Dtos;
using TechMove.Core.Data;
using TechMove.Core.Models;

namespace TechMove.API.Controllers;

/// <summary>
/// API Controller for managing clients
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClientsController(AppDbContext db) : ControllerBase
{
    /// <summary>
    /// Get all clients
    /// </summary>
    /// <returns>List of all clients</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ClientDto>>> GetClients()
    {
        var clients = await db.Clients
            .OrderBy(c => c.Name)
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address
            })
            .ToListAsync();

        return Ok(clients);
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
        var client = await db.Clients
            .Include(c => c.Contracts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound();

        var dto = new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address
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
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Client name is required.");

        var client = new Client
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address
        };

        db.Clients.Add(client);
        await db.SaveChangesAsync();

        var responseDto = new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address
        };

        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, responseDto);
    }

    /// <summary>
    /// Update an existing client
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
        var client = await db.Clients.FindAsync(id);
        if (client == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Client name is required.");

        client.Name = dto.Name;
        client.Email = dto.Email;
        client.Phone = dto.Phone;
        client.Address = dto.Address;

        db.Clients.Update(client);
        await db.SaveChangesAsync();

        var responseDto = new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address
        };

        return Ok(responseDto);
    }

    /// <summary>
    /// Delete a client
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await db.Clients.FindAsync(id);
        if (client == null)
            return NotFound();

        db.Clients.Remove(client);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
