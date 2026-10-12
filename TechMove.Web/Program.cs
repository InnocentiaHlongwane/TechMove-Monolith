using Microsoft.EntityFrameworkCore;
using TechMove.Web.Data;
using TechMove.Web.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Use SQLite by default for local development to avoid SQL Server login issues.
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));

builder.Services.AddHttpClient<CurrencyService>(client => client.Timeout = TimeSpan.FromSeconds(10));
var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();