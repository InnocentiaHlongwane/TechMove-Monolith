using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Core.Models;
using TechMove.Core.Services;
using TechMove.Web.Services;

namespace TechMove.Web.Controllers;

/// <summary>
/// MVC Controller for managing contracts via API calls
/// </summary>
public class ContractsController(IContractApiClient contractClient, IClientApiClient clientClient) : Controller
{
    /// <summary>
    /// Display list of contracts with optional filtering
    /// </summary>
    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
    {
        var contracts = await contractClient.GetContractsAsync(startDate, endDate, status);

        ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
        ViewBag.Status = status;

        return View(contracts);
    }

    /// <summary>
    /// Display create contract form
    /// </summary>
    public async Task<IActionResult> Create()
    {
        await LoadClients();
        return View();
    }

    /// <summary>
    /// Handle contract creation with file upload
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int clientId, DateTime startDate, DateTime endDate, string serviceLevel, IFormFile? signedAgreement)
    {
        // Validate file
        if (signedAgreement is null || signedAgreement.Length == 0)
        {
            ModelState.AddModelError(nameof(signedAgreement), "A signed PDF agreement is required.");
        }
        else if (!UploadValidator.IsValidPdf(signedAgreement.FileName, signedAgreement.ContentType))
        {
            ModelState.AddModelError(nameof(signedAgreement), "Only PDF files are allowed.");
        }

        // Validate dates
        if (endDate < startDate)
        {
            ModelState.AddModelError(nameof(endDate), "End date must be on or after the start date.");
        }

        if (!ModelState.IsValid)
        {
            await LoadClients(clientId);
            return View();
        }

        try
        {
            // Call API to create contract
            var contract = await contractClient.CreateContractAsync(clientId, startDate, endDate, serviceLevel, signedAgreement);

            TempData["SuccessMessage"] = $"Contract {contract.Id} created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating contract: {ex.Message}");
            await LoadClients(clientId);
            return View();
        }
    }

    /// <summary>
    /// Download contract agreement PDF
    /// </summary>
    public async Task<IActionResult> Download(int id)
    {
        try
        {
            var fileBytes = await contractClient.DownloadContractAsync(id);

            if (fileBytes.Length == 0)
                return NotFound("Contract not found.");

            return File(fileBytes, "application/pdf", $"Contract-{id}-Agreement.pdf");
        }
        catch (Exception ex)
        {
            return NotFound($"Error downloading contract: {ex.Message}");
        }
    }

    /// <summary>
    /// Load available clients for dropdown
    /// </summary>
    private async Task LoadClients(int? selected = null)
    {
        var clients = await clientClient.GetClientsAsync();
        ViewBag.ClientId = new SelectList(clients, "Id", "Name", selected);
    }
}
