using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMove.API.Dtos;
using TechMove.Core.Models;
using TechMove.Core.Services;

namespace TechMove.API.Controllers;

/// <summary>
/// API Controller for managing service requests
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ServiceRequestsController(IServiceRequestRepository serviceRequestRepository) : ControllerBase
{
    /// <summary>
    /// Get all service requests
    /// </summary>
    /// <returns>List of service requests</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceRequestDto>>> GetServiceRequests()
    {
        var requests = await serviceRequestRepository.GetAllAsync();
        var dtos = requests.Select(sr => new ServiceRequestDto
        {
            Id = sr.Id,
            ContractId = sr.ContractId,
            Description = sr.Description,
            Cost = sr.Cost,
            Currency = sr.Currency,
            CreatedAt = sr.CreatedAt
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Get service requests by contract ID
    /// </summary>
    /// <param name="contractId">Contract ID</param>
    /// <returns>List of service requests for the contract</returns>
    [HttpGet("contract/{contractId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceRequestDto>>> GetServiceRequestsByContractId(int contractId)
    {
        var requests = await serviceRequestRepository.GetByContractIdAsync(contractId);
        var dtos = requests.Select(sr => new ServiceRequestDto
        {
            Id = sr.Id,
            ContractId = sr.ContractId,
            Description = sr.Description,
            Cost = sr.Cost,
            Currency = sr.Currency,
            CreatedAt = sr.CreatedAt
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Get a specific service request by ID
    /// </summary>
    /// <param name="id">Service request ID</param>
    /// <returns>Service request details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceRequestDto>> GetServiceRequest(int id)
    {
        var request = await serviceRequestRepository.GetByIdAsync(id);
        if (request == null)
            return NotFound();

        var dto = new ServiceRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            Description = request.Description,
            Cost = request.Cost,
            Currency = request.Currency,
            CreatedAt = request.CreatedAt
        };

        return Ok(dto);
    }

    /// <summary>
    /// Create a new service request
    /// </summary>
    /// <param name="dto">Service request details</param>
    /// <returns>Created service request</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceRequestDto>> CreateServiceRequest([FromBody] CreateServiceRequestDto dto)
    {
        var request = new ServiceRequest
        {
            ContractId = dto.ContractId,
            Description = dto.Description,
            Cost = dto.Cost,
            Currency = dto.Currency ?? "ZAR",
            CreatedAt = DateTime.UtcNow
        };

        var created = await serviceRequestRepository.CreateAsync(request);

        var responseDto = new ServiceRequestDto
        {
            Id = created.Id,
            ContractId = created.ContractId,
            Description = created.Description,
            Cost = created.Cost,
            Currency = created.Currency,
            CreatedAt = created.CreatedAt
        };

        return CreatedAtAction(nameof(GetServiceRequest), new { id = created.Id }, responseDto);
    }

    /// <summary>
    /// Update a service request
    /// </summary>
    /// <param name="id">Service request ID</param>
    /// <param name="dto">Updated service request details</param>
    /// <returns>Updated service request</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceRequestDto>> UpdateServiceRequest(int id, [FromBody] CreateServiceRequestDto dto)
    {
        var request = await serviceRequestRepository.GetByIdAsync(id);
        if (request == null)
            return NotFound();

        request.Description = dto.Description;
        request.Cost = dto.Cost;
        request.Currency = dto.Currency ?? request.Currency;

        var updated = await serviceRequestRepository.UpdateAsync(request);

        var responseDto = new ServiceRequestDto
        {
            Id = updated.Id,
            ContractId = updated.ContractId,
            Description = updated.Description,
            Cost = updated.Cost,
            Currency = updated.Currency,
            CreatedAt = updated.CreatedAt
        };

        return Ok(responseDto);
    }

    /// <summary>
    /// Delete a service request
    /// </summary>
    /// <param name="id">Service request ID</param>
    /// <returns>Success indicator</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteServiceRequest(int id)
    {
        var result = await serviceRequestRepository.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
