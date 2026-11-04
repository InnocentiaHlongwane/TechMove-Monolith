using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechMove.Core.Data;

namespace TechMove.Tests.Integration;

/// <summary>
/// WebApplicationFactory for integration testing the TechMove API
/// Uses in-memory SQLite database for isolated testing
/// </summary>
public class TechMoveApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"techmove_test_{Guid.NewGuid()}";

    /// <summary>
    /// Configure services for testing with in-memory database
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the production DbContext
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add in-memory database for testing
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Build service provider and seed database
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();

                // Seed test data if database is empty
                if (!db.Clients.Any())
                {
                    SeedTestData(db);
                }
            }
        });
    }

    /// <summary>
    /// Seed sample data for integration tests
    /// </summary>
    private static void SeedTestData(AppDbContext db)
    {
        // Create test clients
        var client1 = new TechMove.Core.Models.Client
        {
            Name = "Test Client 1",
            ContactDetails = "test1@example.com",
            Region = "South Africa"
        };

        var client2 = new TechMove.Core.Models.Client
        {
            Name = "Test Client 2",
            ContactDetails = "test2@example.com",
            Region = "Gauteng"
        };

        db.Clients.AddRange(client1, client2);
        db.SaveChanges();

        // Create test contracts
        var contract1 = new TechMove.Core.Models.Contract
        {
            ClientId = client1.Id,
            StartDate = DateTime.UtcNow.AddMonths(-1),
            EndDate = DateTime.UtcNow.AddYears(1),
            Status = TechMove.Core.Models.ContractStatus.Active,
            ServiceLevel = "Premium"
        };

        var contract2 = new TechMove.Core.Models.Contract
        {
            ClientId = client2.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(2),
            Status = TechMove.Core.Models.ContractStatus.Draft,
            ServiceLevel = "Standard"
        };

        db.Contracts.AddRange(contract1, contract2);
        db.SaveChanges();

        // Create test service requests
        var serviceRequest1 = new TechMove.Core.Models.ServiceRequest
        {
            ContractId = contract1.Id,
            Description = "Test service request 1",
            Cost = 5000,
            Currency = "ZAR",
            CreatedAt = DateTime.UtcNow
        };

        var serviceRequest2 = new TechMove.Core.Models.ServiceRequest
        {
            ContractId = contract1.Id,
            Description = "Test service request 2",
            Cost = 3000,
            Currency = "ZAR",
            CreatedAt = DateTime.UtcNow
        };

        db.ServiceRequests.AddRange(serviceRequest1, serviceRequest2);
        db.SaveChanges();
    }
}
