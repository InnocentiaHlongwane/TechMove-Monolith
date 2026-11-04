using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Core.Models;
using TechMove.Core.Services;
using TechMove.Web.Services;

namespace TechMove.Web.Controllers;

/// <summary>
/// MVC Controller for managing service requests via API calls
/// </summary>
public class ServiceRequestsController(
    IServiceRequestApiClient serviceRequestClient,
    IClientApiClient clientClient,
    IContractApiClient contractClient,
    CurrencyService currency) : Controller
{
    /// <summary>
    /// Display list of all service requests
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var requests = await serviceRequestClient.GetServiceRequestsAsync();
        return View(requests.OrderByDescending(s => s.CreatedAt).ToList());
    }

    /// <summary>
    /// Display create service request form
    /// </summary>
    public async Task<IActionResult> Create()
    {
        await LoadContracts();

        // Pass contract data as JSON for client-side filtering
        var contracts = await contractClient.GetContractsAsync();
        var activeContracts = contracts
            .Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
            .ToList();

        var contractData = activeContracts.Select(c => new
        {
            id = c.Id,
            clientId = c.ClientId,
            clientName = c.Client?.Name ?? "Unknown",
            display = $"{c.Client?.Name ?? "Unknown"} — #{c.Id} ({c.Status})"
        }).ToList();

        ViewBag.ContractData = System.Text.Json.JsonSerializer.Serialize(contractData);

        try
        {
            ViewBag.UsdToZarRate = await currency.GetUsdToZarRateAsync();
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "Could not retrieve the live exchange rate. Refresh or try again later.");
            ViewBag.UsdToZarRate = 0m;
        }
        catch (InvalidOperationException)
        {
            ModelState.AddModelError("", "Currency service returned an invalid rate.");
            ViewBag.UsdToZarRate = 0m;
        }

        return View();
    }

    /// <summary>
    /// Handle service request creation
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int contractId, string description, decimal usdAmount, string submitCurrency)
    {
        // Validate contract
        var activeContracts = await contractClient.GetContractsAsync();
        var contract = activeContracts.FirstOrDefault(c => c.Id == contractId);

        if (contract == null)
            ModelState.AddModelError(nameof(contractId), "Select a valid contract.");
        else if (!ContractWorkflow.CanCreateServiceRequest(contract.Status))
            ModelState.AddModelError("", $"A service request cannot be created because this contract is {contract.Status}. Choose an Active or Draft contract.");

        if (string.IsNullOrWhiteSpace(description))
            ModelState.AddModelError(nameof(description), "Description is required.");

        if (usdAmount <= 0)
            ModelState.AddModelError(nameof(usdAmount), "Enter a USD amount greater than zero.");

        if (!ModelState.IsValid)
        {
            await LoadContracts(contractId);
            try
            {
                ViewBag.UsdToZarRate = await currency.GetUsdToZarRateAsync();
            }
            catch
            {
                ViewBag.UsdToZarRate = 0m;
            }
            return View();
        }

        decimal rate;
        try
        {
            rate = await currency.GetUsdToZarRateAsync();
        }
        catch
        {
            ModelState.AddModelError("", "Could not verify the current exchange rate. Please try again.");
            await LoadContracts(contractId);
            ViewBag.UsdToZarRate = 0m;
            return View();
        }

        // Server recalculates the amount; never trust a client-posted calculated cost.
        var zar = Math.Round(usdAmount * rate, 2, MidpointRounding.AwayFromZero);

        try
        {
            // Call API to create service request
            var request = await serviceRequestClient.CreateServiceRequestAsync(contractId, description.Trim(), zar, "ZAR");

            TempData["SuccessMessage"] = $"Service request {request.Id} created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating service request: {ex.Message}");
            await LoadContracts(contractId);
            return View();
        }
    }

    /// <summary>
    /// Load available contracts for dropdown
    /// </summary>
    private async Task LoadContracts(int? selected = null)
    {
        var contracts = await contractClient.GetContractsAsync();
        var activeContracts = contracts
            .Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
            .OrderBy(c => c.Client?.Name ?? "Unknown")
            .ToList();

        var selectListItems = activeContracts.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.Client?.Name ?? "Unknown"} — #{c.Id} ({c.Status})",
            Selected = (selected.HasValue && c.Id == selected.Value)
        }).ToList();

        ViewBag.ContractId = new SelectList(selectListItems, "Value", "Text", selected);
    }
}
