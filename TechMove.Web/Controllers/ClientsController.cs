using Microsoft.AspNetCore.Mvc;
using TechMove.Core.Models;
using TechMove.Web.Services;

namespace TechMove.Web.Controllers;

/// <summary>
/// MVC Controller for managing clients via API calls
/// </summary>
public class ClientsController(IClientApiClient clientClient, IContractApiClient contractClient) : Controller
{
    /// <summary>
    /// Display list of all clients
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var clients = await clientClient.GetClientsAsync();
        return View(clients);
    }

    /// <summary>
    /// Display create client form
    /// </summary>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handle client creation
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Client client)
    {
        if (!ModelState.IsValid)
            return View(client);

        try
        {
            // Call API to create client
            var createdClient = await clientClient.CreateClientAsync(client.Name, client.ContactDetails, client.Region);

            TempData["SuccessMessage"] = $"Client {createdClient.Name} created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating client: {ex.Message}");
            return View(client);
        }
    }

    /// <summary>
    /// Display client details and associated contracts
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = await clientClient.GetClientAsync(id);

            if (client == null)
                return NotFound();

            // Also get contracts for this client
            var allContracts = await contractClient.GetContractsAsync();
            var clientContracts = allContracts.Where(c => c.ClientId == id).ToList();

            ViewBag.Contracts = clientContracts;

            return View(client);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error retrieving client: {ex.Message}");
            return NotFound();
        }
    }

    /// <summary>
    /// Display edit client form
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = await clientClient.GetClientAsync(id);

            if (client == null)
                return NotFound();

            return View(client);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error retrieving client: {ex.Message}");
            return NotFound();
        }
    }

    /// <summary>
    /// Handle client update
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Client client)
    {
        if (id != client.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(client);

        try
        {
            var updatedClient = await clientClient.UpdateClientAsync(id, client.Name, client.ContactDetails, client.Region);

            TempData["SuccessMessage"] = $"Client {updatedClient.Name} updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error updating client: {ex.Message}");
            return View(client);
        }
    }

    /// <summary>
    /// Handle client deletion
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await clientClient.DeleteClientAsync(id);

            if (success)
                TempData["SuccessMessage"] = "Client deleted successfully.";
            else
                TempData["ErrorMessage"] = "Error deleting client.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting client: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
