# TechMove MVC Refactoring - API Client & Authentication Implementation

## Overview

This refactoring separates the **Presentation Layer (TechMove.Web MVC)** from the **Service/Data Layer (TechMove.API)** by implementing:

1. **HTTP-based API Clients** - Typed HttpClient services for API communication
2. **JWT Authentication** - Token-based authentication for secure API access
3. **Session-based Token Storage** - Secure handling of JWT tokens on the MVC client

## Architecture Changes

### Before Refactoring
```
TechMove.Web (MVC)
  ├── Direct DbContext Access
  ├── Direct Database Operations
  └── Mixed Presentation & Data Logic
```

### After Refactoring
```
TechMove.Web (MVC)
  ├── Controllers (UI Logic Only)
  ├── API Client Services (HttpClient)
  ├── Authentication Service
  └── Session Token Storage
		 ↓ (HTTPS REST API)
TechMove.API (REST API)
  ├── Controllers (Endpoints)
  ├── Authentication (JWT)
  ├── Repositories (Data Access)
  └── Database (SQLite)
```

## Key Features

### 1. API Client Services

Three typed HTTP clients replace direct database access:

- **`IContractApiClient`** - Manages contracts via `api/contracts` endpoints
- **`IClientApiClient`** - Manages clients via `api/clients` endpoints
- **`IServiceRequestApiClient`** - Manages service requests via `api/servicerequests` endpoints

**Benefits:**
- Centralized HTTP communication logic
- Consistent error handling and logging
- Automatic token injection via `AuthenticatedHttpClientHandler`
- Dependency injection support

### 2. Authentication Flow

#### Login Process
```
User Login Form (MVC)
	↓
LoginController.Login()
	↓
POST /api/auth/login (API)
	↓
JWT Token Generated
	↓
Token Stored in Session
	↓
Redirect to Dashboard
```

#### Protected API Requests
```
MVC Controller Action
	↓
Inject IContractApiClient (or other)
	↓
Service HttpClient sends request
	↓
AuthenticatedHttpClientHandler retrieves token from session
	↓
Adds "Authorization: Bearer {token}" header
	↓
API receives request with token
	↓
[Authorize] validates token
	↓
API returns data (or 401 if invalid token)
```

#### Demo Credentials
```
Username: admin     Password: admin123
Username: user      Password: user123
Username: lebo      Password: password123
```

## Project Structure

### TechMove.Web Changes

**New Files:**
- `Services/IAuthenticationService.cs` - Token management interface
- `Services/AuthenticationService.cs` - Session-based token storage
- `Services/AuthenticatedHttpClientHandler.cs` - Auto-injects JWT to API requests
- `Services/IContractApiClient.cs` - Contract API client interface
- `Services/ContractApiClient.cs` - Contract HTTP client implementation
- `Services/IClientApiClient.cs` - Client API client interface
- `Services/ClientApiClient.cs` - Client HTTP client implementation
- `Services/IServiceRequestApiClient.cs` - Service request API client interface
- `Services/ServiceRequestApiClient.cs` - Service request HTTP client implementation
- `Controllers/LoginController.cs` - Authentication controller
- `Controllers/ContractsController.cs` - Refactored to use API client
- `Controllers/ClientsController.cs` - Refactored to use API client
- `Controllers/ServiceRequestsController.cs` - Refactored to use API client

**Modified Files:**
- `Program.cs` - Added session configuration, authentication services, and API client registration with message handler
- `appsettings.json` - Changed from database connection to API base URL

**Removed Functionality:**
- Direct Entity Framework DbContext usage
- Database seeding in MVC startup

### TechMove.API Changes

**New Files:**
- `Controllers/AuthController.cs` - JWT token generation endpoint

**Modified Files:**
- `Program.cs` - Added JWT authentication configuration, bearer scheme setup
- `Controllers/ContractsController.cs` - Added `[Authorize]` attribute
- `Controllers/ClientsController.cs` - Added `[Authorize]` attribute
- `Controllers/ServiceRequestsController.cs` - Added `[Authorize]` attribute
- `appsettings.json` - Added JWT configuration section

**New Packages:**
- `Microsoft.AspNetCore.Authentication.JwtBearer` (v8.0.0)
- `System.IdentityModel.Tokens.Jwt` (v7.0.3)

## Configuration

### MVC Configuration (appsettings.json)
```json
{
  "ApiSettings": {
	"BaseUrl": "https://localhost:56373"
  },
  "Logging": { ... }
}
```

### API Configuration (appsettings.json)
```json
{
  "JwtSettings": {
	"SecretKey": "your-secret-key-change-this-in-production-to-at-least-32-characters",
	"Issuer": "TechMove",
	"Audience": "TechMoveApp",
	"ExpirationMinutes": 60
  },
  "ConnectionStrings": {
	"DefaultConnection": "Data Source=techmove.db"
  }
}
```

## Running the Application

### Step 1: Start the API
```powershell
cd TechMove.API
dotnet run
# API runs at https://localhost:56373
# Swagger at https://localhost:56373/swagger
```

### Step 2: Start the MVC Web App
```powershell
cd ../TechMove.Web
dotnet run
# Web runs at https://localhost:port (typically 7xxx range)
```

### Step 3: Login
1. Navigate to the web application
2. Click "Login" or go to `/login`
3. Use demo credentials above
4. After login, token is stored in session
5. Navigate to Contracts, Clients, or Service Requests

## API Endpoints

### Authentication
- `POST /api/auth/login` - Get JWT token (Authorization: None required)

### Contracts (Authorization: Bearer token required)
- `GET /api/contracts` - List contracts with optional filters
- `GET /api/contracts/{id}` - Get specific contract
- `POST /api/contracts` - Create new contract
- `PATCH /api/contracts/{id}/status` - Update contract status
- `GET /api/contracts/{id}/download` - Download contract PDF

### Clients (Authorization: Bearer token required)
- `GET /api/clients` - List all clients
- `GET /api/clients/{id}` - Get specific client
- `POST /api/clients` - Create new client
- `PUT /api/clients/{id}` - Update client
- `DELETE /api/clients/{id}` - Delete client

### Service Requests (Authorization: Bearer token required)
- `GET /api/servicerequests` - List all service requests
- `GET /api/servicerequests/contract/{contractId}` - List by contract
- `GET /api/servicerequests/{id}` - Get specific request
- `POST /api/servicerequests` - Create new request
- `PUT /api/servicerequests/{id}` - Update request
- `DELETE /api/servicerequests/{id}` - Delete request

## Error Handling

### MVC Side
- **API Connection Issues** - Friendly error messages displayed to users
- **Authentication Failures** - Redirect to login page
- **Network Timeouts** - 30-second timeout with retry capability

### API Side
- **Invalid Token** - Returns `401 Unauthorized`
- **Expired Token** - Returns `401 Unauthorized`
- **Database Errors** - Returns `500 Internal Server Error` with logging

## Security Considerations

### Current Implementation (Development)
- Session-based token storage (HTTP-only recommended in production)
- Simple demo authentication (replace with real user database)
- CORS allows all origins (restrict in production)
- Demo JWT secret key (use strong key and environment variables)

### Production Recommendations
1. **Use secure, HTTP-only cookies** instead of session for token storage
2. **Implement proper user database** with password hashing (bcrypt/Argon2)
3. **Restrict CORS** to specific domains
4. **Use environment variables** for JWT secrets
5. **Add HTTPS enforcement** (already enabled)
6. **Implement token refresh** mechanism
7. **Add rate limiting** to auth endpoint
8. **Use strong secret keys** (32+ characters with mixed case, numbers, symbols)

## Testing

### Test Login
```powershell
$body = @{username="admin"; password="admin123"} | ConvertTo-Json
Invoke-RestMethod -Uri "https://localhost:56373/api/auth/login" -Method Post -Body $body -ContentType "application/json"
```

### Test Protected Endpoint
```powershell
$token = "eyJhbGciOiJIUzI1NiIs..." # Replace with token from login
$headers = @{Authorization = "Bearer $token"}
Invoke-RestMethod -Uri "https://localhost:56373/api/contracts" -Headers $headers
```

## Troubleshooting

### "Cannot connect to API"
- Ensure API is running on port 56373
- Check `ApiSettings:BaseUrl` in MVC appsettings.json
- Verify HTTPS certificate trust

### "401 Unauthorized"
- Token may have expired (60-minute default lifespan)
- Log out and log in again
- Check JWT secret key matches between MVC and API

### "CORS error"
- API CORS is set to allow all origins in development
- Check browser console for specific CORS error
- API is configured to run on same HTTPS scheme

### Session token not persisting
- Ensure `UseSession()` middleware is added after `UseRouting()`
- Session requires distributed memory cache or other backing store
- Default uses in-memory cache (not suitable for multi-server production)

## Next Steps for Production

1. Replace demo authentication with real user database
2. Implement token refresh mechanism
3. Add audit logging for all API operations
4. Implement proper error tracking (Application Insights, Sentry, etc.)
5. Add API rate limiting and throttling
6. Implement database migrations strategy
7. Add integration and load tests
8. Configure environment-specific settings
9. Set up CI/CD pipeline
10. Implement API versioning strategy

## Dependencies

**TechMove.Web:**
- Microsoft.AspNetCore.* (framework)
- System.Net.Http (HTTP client)
- System.Text.Json (JSON serialization)

**TechMove.API:**
- Microsoft.AspNetCore.Authentication.JwtBearer (v8.0.0)
- System.IdentityModel.Tokens.Jwt (v7.0.3)
- Microsoft.EntityFrameworkCore (v8.0.11)

## References

- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication)
- [JWT Bearer Authentication](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer)
- [HttpClient Best Practices](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security)
