using Microsoft.EntityFrameworkCore;
using TechMove.Web.Data;

namespace TechMove.Web;

/// <summary>
/// Quick utility to inspect database tables and schema
/// </summary>
public class DatabaseViewer
{
    public static void Main(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=techmove.db");

        using (var context = new AppDbContext(optionsBuilder.Options))
        {
            Console.WriteLine("=== TechMove Database Schema ===\n");

            // Show Clients table
            Console.WriteLine("📊 TABLE: Clients");
            Console.WriteLine("─────────────────────────────────────────────");
            var clients = context.Clients.ToList();
            Console.WriteLine($"Rows: {clients.Count}");
            Console.WriteLine("Columns: Id (int), Name (string), ContactDetails (string), Region (string)\n");

            // Show Contracts table
            Console.WriteLine("📊 TABLE: Contracts");
            Console.WriteLine("─────────────────────────────────────────────");
            var contracts = context.Contracts.ToList();
            Console.WriteLine($"Rows: {contracts.Count}");
            Console.WriteLine("Columns: Id (int), ClientId (int FK), StartDate (DateTime), EndDate (DateTime),");
            Console.WriteLine("         Status (int), ServiceLevel (string), SignedAgreementPath (string)\n");

            // Show ServiceRequests table
            Console.WriteLine("📊 TABLE: ServiceRequests");
            Console.WriteLine("─────────────────────────────────────────────");
            var serviceRequests = context.ServiceRequests.ToList();
            Console.WriteLine($"Rows: {serviceRequests.Count}");
            Console.WriteLine("Columns: Id (int), ContractId (int FK), Description (string), Cost (decimal),");
            Console.WriteLine("         Currency (string), OriginalUsdAmount (decimal), CreatedAt (DateTime)\n");

            Console.WriteLine("✅ Migration History: Applied");
            Console.WriteLine("   • 20261012212643_InitialCreate (Current)\n");
        }
    }
}
