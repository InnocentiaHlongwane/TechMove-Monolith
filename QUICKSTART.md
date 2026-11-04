# Quick Start Guide - TechMove API with Swagger

## Starting the API

### Option 1: Using Visual Studio
1. Open `TechMoveMonolith.sln` in Visual Studio
2. Set `TechMove.API` as the startup project
3. Press `F5` or click **Start**
4. The API will open at `https://localhost:56373`
5. Navigate to `/swagger/ui` to access Swagger UI

### Option 2: Using Command Line
```powershell
cd C:\Users\Lebogang\source\repos\TechMoveMonolith
dotnet run --project TechMove.API
# API runs at https://localhost:56373
```

## Accessing Swagger Documentation

Once the API is running, open your browser and go to:

```
https://localhost:56373/swagger/ui
```

You'll see an interactive interface where you can:
- View all available endpoints
- See request/response schemas
- Try endpoints directly without coding
- Get automatic curl commands

## Testing Sample Endpoints

### 1. Get All Contracts
```bash
curl -X GET "https://localhost:56373/api/contracts" -H "accept: application/json"
```

### 2. Create a Client (simpler test)
```bash
curl -X POST "https://localhost:56373/api/clients" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
	"name": "Test Client",
	"contactDetails": "contact@test.com",
	"region": "Test Region"
  }'
```

### 3. Get Specific Client
```bash
curl -X GET "https://localhost:56373/api/clients/1" -H "accept: application/json"
```

### 4. Create Service Request
```bash
curl -X POST "https://localhost:56373/api/servicerequests" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
	"contractId": 1,
	"description": "Website maintenance",
	"cost": 5000.00,
	"currency": "ZAR"
  }'
```

## Database

The API uses SQLite by default:
- **Database File**: `techmove.db` (created on first run)
- **Location**: Root of the TechMove.API project
- **Automatic**: Database is created, migrations are applied, and sample data is seeded on startup

## Swagger Features

### View API Documentation
- Click on any endpoint to expand it
- See HTTP method, URL, parameters, and responses
- View status codes and error descriptions

### Try It Out
- Click the **"Try it out"** button
- Fill in parameters
- Click **"Execute"** to make the request
- See the response in real-time

### Download OpenAPI Spec
- At the top of Swagger UI: **"API"** dropdown → **"Download OpenAPI specification"**
- This downloads the machine-readable API definition (JSON)

## Architecture Overview

```
TechMove Solution
├── TechMove.Web (MVC Application)
│   └── Controllers: HomeController, etc.
│       └── Views: Razor templates
├── TechMove.API (REST API) ← NEW
│   ├── Controllers: ContractsController, ClientsController, ServiceRequestsController
│   ├── Services: Repository implementations
│   └── DTOs: Data transfer objects
└── TechMove.Core (Shared)
	├── Models: Contract, Client, ServiceRequest
	├── Services: Business logic
	└── Data: AppDbContext, Migrations
```

## What Each Controller Does

### Contracts Controller (`/api/contracts`)
Manages contracts including filtering by date and status
- List contracts with optional filtering
- Create contracts with PDF uploads
- Update contract status (approve/decline)
- Download signed agreements

### Clients Controller (`/api/clients`)
Manages client information
- List all clients
- Get client details
- Create new clients
- Update client information
- Delete clients

### Service Requests Controller (`/api/servicerequests`)
Manages service requests
- List all service requests
- Get requests by contract
- Create new requests
- Update requests
- Delete requests

## Configuration

### appsettings.json
Located in `TechMove.API/`

Current settings:
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Data Source=techmove.db"
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  }
}
```

### Changing Port
Edit `TechMove.API/Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:YOUR_PORT;http://localhost:YOUR_HTTP_PORT"
```

## Troubleshooting

### Swagger UI Not Loading
- Ensure you're in **Development** environment
- Check that API is running on the correct port
- Clear browser cache

### Database Not Found
- The database is auto-created on first run
- Check file permissions in the TechMove.API directory
- Delete `techmove.db` to reset the database

### CORS Errors
- Swagger UI itself should work fine
- If consuming from other domains, update CORS policy in Program.cs:
```csharp
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecific", builder =>
	{
		builder.WithOrigins("https://yourdomain.com")
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});
```

## API Response Format

### Success Response (200 OK)
```json
{
  "id": 1,
  "name": "Client Name",
  "contactDetails": "email@example.com",
  "region": "South Africa"
}
```

### Error Response (400 Bad Request)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "..."
}
```

## Next Steps

1. **Test Endpoints**: Use Swagger UI to test all endpoints
2. **Integrate with Frontend**: Update TechMove.Web to call the API
3. **Add Authentication**: Implement JWT or OAuth2
4. **Deploy**: Deploy API to production server
5. **Monitor**: Setup logging and monitoring

## Resources

- [Swagger/OpenAPI Documentation](https://swagger.io/)
- [ASP.NET Core Web API Best Practices](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Swashbuckle Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

---

**API Status**: ✅ Ready to Use
**Swagger Status**: ✅ Enabled and Documented
**Build Status**: ✅ Successful
