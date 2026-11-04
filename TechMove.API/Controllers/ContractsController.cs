using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.API.Dtos;
using TechMove.Core.Data;
using TechMove.Core.Models;
using TechMove.Core.Services;

namespace TechMove.API.Controllers;

/// <summary>
/// API Controller for managing contracts
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContractsController(AppDbContext db, IWebHostEnvironment env) : ControllerBase
{
    /// <summary>
    /// Get all contracts with optional filtering
    /// </summary>
    /// <param name="startDate">Filter contracts starting from this date (optional)</param>
    /// <param name="endDate">Filter contracts ending before this date (optional)</param>
    /// <param name="status">Filter by contract status (optional)</param>
    /// <returns>List of contracts</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContractDto>>> GetContracts(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? status)
    {
        var query = db.Contracts.Include(c => c.Client).AsQueryable();

        if (startDate.HasValue)
            query = query.Where(c => c.StartDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(c => c.EndDate <= endDate.Value);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ContractStatus>(status, true, out var statusEnum))
            query = query.Where(c => c.Status == statusEnum);

        var contracts = await query
            .OrderByDescending(c => c.StartDate)
            .Select(c => new ContractDto
            {
                Id = c.Id,
                ClientId = c.ClientId,
                ClientName = c.Client!.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Status = c.Status.ToString(),
                ServiceLevel = c.ServiceLevel,
                SignedAgreementPath = c.SignedAgreementPath
            })
            .ToListAsync();

        return Ok(contracts);
    }

    /// <summary>
    /// Get a specific contract by ID
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <returns>Contract details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractDto>> GetContract(int id)
    {
        var contract = await db.Contracts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
            return NotFound();

        var dto = new ContractDto
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            ClientName = contract.Client?.Name,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Status = contract.Status.ToString(),
            ServiceLevel = contract.ServiceLevel,
            SignedAgreementPath = contract.SignedAgreementPath
        };

        return Ok(dto);
    }

    /// <summary>
    /// Create a new contract
    /// </summary>
    /// <param name="dto">Contract creation details</param>
    /// <param name="signedAgreement">PDF file of the signed agreement</param>
    /// <returns>Created contract</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContractDto>> CreateContract(
        [FromForm] CreateContractDto dto,
        [FromForm] IFormFile? signedAgreement)
    {
        // Validate file upload
        if (signedAgreement == null || signedAgreement.Length == 0)
            return BadRequest("A signed PDF agreement is required.");

        if (!UploadValidator.IsValidPdf(signedAgreement.FileName, signedAgreement.ContentType))
            return BadRequest("Only PDF files are allowed.");

        // Validate date range
        if (dto.EndDate < dto.StartDate)
            return BadRequest("End date must be on or after the start date.");

        // Verify client exists
        var client = await db.Clients.FindAsync(dto.ClientId);
        if (client == null)
            return BadRequest("Invalid client ID.");

        // Save the file
        var uploadFolder = Path.Combine(env.WebRootPath ?? "uploads", "contracts");
        Directory.CreateDirectory(uploadFolder);
        var fileName = $"{Guid.NewGuid():N}.pdf";
        var filePath = Path.Combine(uploadFolder, fileName);

        await using (var stream = System.IO.File.Create(filePath))
            await signedAgreement.CopyToAsync(stream);

        // Create contract
        var contract = new Contract
        {
            ClientId = dto.ClientId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ServiceLevel = dto.ServiceLevel,
            SignedAgreementPath = $"uploads/contracts/{fileName}",
            Status = ContractStatus.Draft
        };

        db.Contracts.Add(contract);
        await db.SaveChangesAsync();

        var responseDto = new ContractDto
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            ClientName = client.Name,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Status = contract.Status.ToString(),
            ServiceLevel = contract.ServiceLevel,
            SignedAgreementPath = contract.SignedAgreementPath
        };

        return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, responseDto);
    }

    /// <summary>
    /// Update contract status (approve/decline/change state)
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <param name="dto">New status</param>
    /// <returns>Updated contract</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContractDto>> UpdateContractStatus(
        int id,
        [FromBody] UpdateContractStatusDto dto)
    {
        var contract = await db.Contracts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null)
            return NotFound();

        if (!Enum.TryParse<ContractStatus>(dto.Status, true, out var newStatus))
            return BadRequest("Invalid status value.");

        // Validate state transition
        if (!ContractWorkflow.CanTransitionTo(contract.Status, newStatus))
            return BadRequest($"Cannot transition from {contract.Status} to {newStatus}.");

        contract.Status = newStatus;
        await db.SaveChangesAsync();

        var responseDto = new ContractDto
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            ClientName = contract.Client?.Name,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Status = contract.Status.ToString(),
            ServiceLevel = contract.ServiceLevel,
            SignedAgreementPath = contract.SignedAgreementPath
        };

        return Ok(responseDto);
    }

    /// <summary>
    /// Download signed contract agreement
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <returns>PDF file</returns>
    [HttpGet("{id}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAgreement(int id)
    {
        var contract = await db.Contracts.FindAsync(id);
        if (contract == null)
            return NotFound("Contract not found.");

        var fullPath = Path.Combine(
            env.WebRootPath ?? ".",
            contract.SignedAgreementPath.Replace('/', Path.DirectorySeparatorChar));

        if (!System.IO.File.Exists(fullPath))
            return NotFound("Agreement file not found.");

        var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        return File(fileBytes, "application/pdf", $"Contract-{contract.Id}-Agreement.pdf");
    }
}
