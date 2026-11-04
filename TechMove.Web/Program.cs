using Microsoft.EntityFrameworkCore;
using TechMove.Core.Data;
using TechMove.Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Use SQLite by default for local development to avoid SQL Server login issues.
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));

builder.Services.AddHttpClient<CurrencyService>(client => client.Timeout = TimeSpan.FromSeconds(10));
var app = builder.Build();

// Apply migrations on startup and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Seed sample data if database is empty
    if (!db.Clients.Any())
    {
        var client = new TechMove.Core.Models.Client
        {
            Name = "Lebo",
            ContactDetails = "lebo@example.com",
            Region = "South Africa"
        };
        db.Clients.Add(client);
        await db.SaveChangesAsync();

        var contract = new TechMove.Core.Models.Contract
        {
            ClientId = client.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            Status = TechMove.Core.Models.ContractStatus.Active,
            ServiceLevel = "Premium"
        };
        db.Contracts.Add(contract);
        await db.SaveChangesAsync();
    }
}
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();