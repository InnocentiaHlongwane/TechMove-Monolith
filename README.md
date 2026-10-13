# TechMove Monolith

A .NET 8 Razor Pages application for managing client contracts and service requests with a focus on currency conversion and service level tracking.

## Project Overview

TechMove is a contract and service request management system built with modern .NET technologies. It enables tracking of client relationships, contract lifecycles, and associated service requests with currency conversion capabilities.

## Technology Stack

- Framework: .NET 8
- UI: Razor Pages
- Database: SQLite with Entity Framework Core
- Database Management: EF Core Migrations
- HTTP Client: HttpClient for external service calls
- Currency Services: Real-time currency conversion integration

## Project Structure

```
TechMoveMonolith/
├── TechMove.Web/                    Main web application
│   ├── Controllers/                 Page controllers
│   ├── Data/                        DbContext and data layer
│   │   └── AppDbContext.cs
│   ├── Models/                      Entity models
│   │   ├── Client.cs
│   │   ├── Contract.cs
│   │   └── ServiceRequest.cs
│   ├── Services/                    Business logic services
│   │   ├── ContractWorkflow.cs
│   │   ├── CurrencyService.cs
│   │   └── UploadValidator.cs
│   ├── Views/                       Razor Pages views
│   │   ├── Clients/
│   │   ├── Contracts/
│   │   ├── ServiceRequests/
│   │   └── Home/
│   ├── Migrations/                  EF Core migrations
│   ├── wwwroot/                     Static files (CSS, JS, uploads)
│   ├── appsettings.json
│   ├── Program.cs
│   └── TechMove.Web.csproj
├── TechMove.Tests/                  Unit tests
│   └── FileValidationTests.cs
└── TechMoveMonolith.sln
```

## Database Schema

### Tables

#### Clients
Stores client information for contract management.
- Id (int, Primary Key)
- Name (string, max 120 chars)
- ContactDetails (string, max 200 chars)
- Region (string, max 100 chars)

#### Contracts
Manages contracts linked to clients with service level details.
- Id (int, Primary Key)
- ClientId (int, Foreign Key to Clients)
- StartDate (DateTime)
- EndDate (DateTime)
- Status (int)
- ServiceLevel (string, max 100 chars)
- SignedAgreementPath (string)

#### ServiceRequests
Tracks service requests associated with contracts, including cost and currency information.
- Id (int, Primary Key)
- ContractId (int, Foreign Key to Contracts)
- Description (string, max 1000 chars)
- Cost (decimal, precision 18,2)
- Currency (string, max 3 chars)
- OriginalUsdAmount (decimal, precision 18,2, nullable)
- CreatedAt (DateTime)

### Relationships
- Contracts to Clients: Many-to-One (ON DELETE RESTRICT)
- ServiceRequests to Contracts: Many-to-One (ON DELETE RESTRICT)

### Indexes
- IX_Contracts_ClientId
- IX_ServiceRequests_ContractId

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2026 (Community or higher) or any .NET-compatible IDE
- PowerShell or Command Prompt

### Installation

1. Clone the repository
   git clone https://github.com/InnocentiaHlongwane/TechMove-Monolith.git
   cd TechMoveMonolith

2. Navigate to the project
   cd TechMove.Web

3. Restore NuGet packages
   dotnet restore

4. Build the project
   dotnet build

5. Run the application
   dotnet run

The application will:
- Start on https://localhost:5001 (or http://localhost:5000)
- Automatically apply EF Core migrations on startup
- Create the SQLite database (techmove.db) if it does not exist

## Database Migrations

This project uses Entity Framework Core Migrations for database versioning and deployment.

### Current Migrations
- 20261012212643_InitialCreate - Initial schema with Clients, Contracts, and ServiceRequests tables

### Creating New Migrations

When you modify the database models:

1. Add migration
   cd TechMove.Web
   dotnet-ef migrations add YourMigrationName

2. Review the generated migration in the Migrations/ folder

3. Run the application to apply the migration

### Generating SQL Scripts

For deployment to production databases:

   dotnet-ef migrations script -o "Migrations/deployment.sql"

This generates pure SQL (useful for manual database deployments).

## Testing

Run unit tests:

   cd TechMove.Tests
   dotnet test

Test Files:
- FileValidationTests.cs - Tests for file upload validation logic

## Features

### Client Management
- Create, read, update, and delete client records
- Track client contact details and regions
- View all active clients

### Contract Management
- Create contracts linked to clients
- Set service levels and contract dates
- Track contract status
- Upload signed agreements

### Service Requests
- Create service requests for contracts
- Track costs with currency support
- Automatic USD amount calculation
- View service request history

### Currency Services
- Real-time currency conversion
- Multi-currency support
- Configurable conversion service

## Configuration

Edit appsettings.json to configure:

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
  },
  "AllowedHosts": "*"
}
```

Database Connection Strings:
- SQLite: Data Source=techmove.db (default)
- SQL Server: Server=.;Database=TechMoveDb;Trusted_Connection=true;

## File Upload

- Uploaded files are stored in: wwwroot/uploads/
- File validation is performed via UploadValidator service
- Supported file types and size limits configured in validation logic

## Security Considerations

- Foreign key constraints enforce referential integrity (ON DELETE RESTRICT)
- Input validation on all user-facing forms
- File upload validation and storage outside webroot recommended for production

## Troubleshooting

### Database Issues
- Database locked: Close other connections and restart the application
- Migration conflicts: Remove the migration and create a new one
- Schema mismatch: Run migrations again: dotnet ef database update

### Connection Issues
- Verify techmove.db exists in TechMove.Web/ directory
- Check connection string in appsettings.json

## Future Enhancements

- Implement authentication and authorization
- Add API endpoints (REST/GraphQL)
- Historical reporting and analytics
- Multi-language support
- Azure deployment configuration
- Advanced filtering and search

## Contributing

1. Create a feature branch
   git checkout -b feature/YourFeature

2. Commit your changes
   git commit -m "Add YourFeature"

3. Push to GitHub
   git push origin feature/YourFeature

4. Open a Pull Request

## License

MIT License - See LICENSE file for details

## Author

Innocentia Hlongwane
Email: ST10544987@rcconnect.edu.za
GitHub: https://github.com/InnocentiaHlongwane

## Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Check existing documentation
- Review migration scripts in Migrations/ folder

---

Last Updated: October 12, 2026
Version: 1.0.0
.NET Target: Net8.0
