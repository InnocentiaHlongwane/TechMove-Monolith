using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Core.Data;
using TechMove.Core.Models;
using TechMove.Core.Services;

namespace TechMove.Web.Controllers;

public class ServiceRequestsController(AppDbContext db, CurrencyService currency) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.ServiceRequests
            .Include(s => s.Contract)!
            .ThenInclude(c => c!.Client)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync());

    public async Task<IActionResult> Create()
    {
        await LoadClients();
        await LoadContracts();

        // Pass contract data as JSON for client-side filtering
        var contracts = await db.Contracts
            .Include(c => c.Client)
            .Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
            .OrderBy(c => c.Client!.Name)
            .ToListAsync();

        var contractData = contracts.Select(c => new 
        { 
            id = c.Id, 
            clientId = c.Client!.Id,
            clientName = c.Client!.Name,
            display = $"{c.Client!.Name} — #{c.Id} ({c.Status})"
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

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int contractId, string description, decimal usdAmount, string submitCurrency)
    {
        var contract = await db.Contracts.FindAsync(contractId);
        if (contract is null)
            ModelState.AddModelError(nameof(contractId), "Select a valid contract.");
        else if (!ContractWorkflow.CanCreateServiceRequest(contract.Status))
            ModelState.AddModelError("", $"A service request cannot be created because this contract is {contract.Status}. Choose an Active or Draft contract.");

        if (string.IsNullOrWhiteSpace(description))
            ModelState.AddModelError(nameof(description), "Description is required.");

        if (usdAmount <= 0)
            ModelState.AddModelError(nameof(usdAmount), "Enter a USD amount greater than zero.");

        if (!ModelState.IsValid)
        {
            await LoadClients();
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
        var request = new ServiceRequest
        {
            ContractId = contractId,
            Description = description.Trim(),
            OriginalUsdAmount = usdAmount,
            Cost = zar,
            Currency = "ZAR",
            CreatedAt = DateTime.UtcNow
        };
        db.ServiceRequests.Add(request);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadClients()
    {
        var clients = await db.Clients
            .OrderBy(c => c.Name)
            .ToListAsync();
        ViewBag.ClientId = new SelectList(clients, "Id", "Name");
    }

    private async Task LoadContracts(int? selected = null)
    {
        var contracts = await db.Contracts
            .Include(c => c.Client)
            .Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
            .OrderBy(c => c.Client!.Name)
            .ToListAsync();
        ViewBag.ContractId = new SelectList(
            contracts.Select(c => new { c.Id, Display = $"{c.Client!.Name} — #{c.Id} ({c.Status})" }),
            "Id",
            "Display",
            selected);
    }
}