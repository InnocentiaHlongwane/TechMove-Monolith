using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.API.Dtos;
using TechMove.Core.Data;
using TechMove.Core.Models;
using TechMove.Core.Services;

namespace TechMove.API.Controllers;

/// <summary>
/// API Controller for managing service requests
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ServiceRequestsController(AppDbContext db) : ControllerBase
{
    /// <summary>
    /// Get all service requests
    /// </summary>
    /// <returns>List of all service requests</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceRequestDto>>> GetServiceRequests()
    {
        var requests = await db.ServiceRequests
            .OrderByDescending(sr => sr.CreatedAt)
            .Select(sr => new ServiceRequestDto
            {
                Id = sr.Id,
                ContractId = sr.ContractId,
                Title = sr.Title,
                Description = sr.Description,
                CreatedAt = sr.CreatedAt,
                Status = sr.Status
            })
            .ToListAsync();

        return Ok(requests);
    }

    /// <summary>
    /// Get all service requests for a specific contract
    /// </summary>
    /// <param name="contractId">Contract ID</param>
    /// <returns>List of service requests for the contract</returns>
    [HttpGet("by-contract/{contractId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ServiceRequestDto>>> GetServiceRequestsByContract(int contractId)
    {
        // Verify contract exists
        var contract = await db.Contracts.FindAsync(contractId);
        if (contract == null)
            return NotFound("Contract not found.");

        var requests = await db.ServiceRequests
            .Where(sr => sr.ContractId == contractId)
            .OrderByDescending(sr => sr.CreatedAt)
            .Select(sr => new ServiceRequestDto
            {
                Id = sr.Id,
                ContractId = sr.ContractId,
                Title = sr.Title,
                Description = sr.Description,
                CreatedAt = sr.CreatedAt,
                Status = sr.Status
            })
            .ToListAsync();

        return Ok(requests);
    }

    /// <summary>
    /// Get a specific service request
    /// </summary>
    /// <param name="id">Service request ID</param>
    /// <returns>Service request details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceRequestDto>> GetServiceRequest(int id)
    {
        var request = await db.ServiceRequests.FirstOrDefaultAsync(sr => sr.Id == id);

        if (request == null)
            return NotFound();

        var dto = new ServiceRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = request.CreatedAt,
            Status = request.Status
        };

        return Ok(dto);
    }

    /// <summary>
    /// Create a new service request for a contract
    /// </summary>
    /// <param name="dto">Service request details</param>
    /// <returns>Created service request</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceRequestDto>> CreateServiceRequest([FromBody] CreateServiceRequestDto dto)
    {
        // Verify contract exists
        var contract = await db.Contracts.FindAsync(dto.ContractId);
        if (contract == null)
            return BadRequest("Invalid contract ID.");

        // Verify contract status allows service requests
        if (!ContractWorkflow.CanCreateServiceRequest(contract.Status))
            return BadRequest($"Cannot create service requests for contracts with status: {contract.Status}");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Service request title is required.");

        var request = new ServiceRequest
        {
            ContractId = dto.ContractId,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            Status = "Open"
        };

        db.ServiceRequests.Add(request);
        await db.SaveChangesAsync();

        var responseDto = new ServiceRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = request.CreatedAt,
            Status = request.Status
        };

        return CreatedAtAction(nameof(GetServiceRequest), new { id = request.Id }, responseDto);
    }

    /// <summary>
    /// Update a service request
    /// </summary>
    /// <param name="id">Service request ID</param>
    /// <param name="dto">Updated details</param>
    /// <returns>Updated service request</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceRequestDto>> UpdateServiceRequest(int id, [FromBody] CreateServiceRequestDto dto)
    {
        var request = await db.ServiceRequests.FindAsync(id);
        if (request == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Service request title is required.");

        request.Title = dto.Title;
        request.Description = dto.Description;

        db.ServiceRequests.Update(request);
        await db.SaveChangesAsync();

        var responseDto = new ServiceRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = request.CreatedAt,
            Status = request.Status
        };

        return Ok(responseDto);
    }

    /// <summary>
    /// Delete a service request
    /// </summary>
    /// <param name="id">Service request ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteServiceRequest(int id)
    {
        var request = await db.ServiceRequests.FindAsync(id);
        if (request == null)
            return NotFound();

        db.ServiceRequests.Remove(request);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
