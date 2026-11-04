# Getting Started - MVC + API Refactored Architecture

## 🚀 Quick Start (2 Minutes)

### Step 1: Start the API
```powershell
cd .\TechMove.API
dotnet run
# Wait for: "Now listening on: https://localhost:56373"
```

### Step 2: Start the Web App (New Terminal)
```powershell
cd .\TechMove.Web
dotnet run
# Wait for: "Now listening on: https://localhost:..."
```

### Step 3: Login
- Open the Web app URL
- Click **Login**
- Use: **admin** / **admin123**
- ✅ You're logged in!

## 📱 What Changed?

### MVC (TechMove.Web)
**Before:** Direct database access via Entity Framework
```csharp
var contracts = await _db.Contracts.ToListAsync(); ❌
```

**Now:** API client calls
```csharp
var contracts = await _contractClient.GetContractsAsync(); ✅
```

### API (TechMove.API)
**Before:** No authentication
**Now:** JWT tokens required
```csharp
[Authorize]
public class ContractsController { } ✅
```

## 🔐 Authentication

### Login Credentials (Demo)
- admin / admin123
- user / user123  
- lebo / password123

### How It Works
1. User logs in → POST /api/auth/login
2. API returns JWT token
3. Token stored in session
4. All API calls include token automatically

## 📊 Test Everything

### In Browser
- **Login:** https://localhost:[port]/login
- **Contracts:** https://localhost:[port]/contracts
- **Clients:** https://localhost:[port]/clients
- **Requests:** https://localhost:[port]/servicerequests

### With PowerShell (Advanced)
```powershell
# Get token
$body = @{username="admin"; password="admin123"} | ConvertTo-Json
$res = Invoke-RestMethod -Uri "https://localhost:56373/api/auth/login" `
	-Method Post -Body $body -ContentType "application/json" -SkipCertificateCheck
$token = $res.token

# Use token
$headers = @{Authorization = "Bearer $token"}
Invoke-RestMethod -Uri "https://localhost:56373/api/contracts" `
	-Headers $headers -SkipCertificateCheck | ConvertTo-Json
```

## 🐛 Troubleshooting

| Problem | Fix |
|---------|-----|
| Can't connect to API | Ensure API runs first at https://localhost:56373 |
| 401 errors | Log out & back in, token expired |
| CORS error | Run both apps, API has AllowAll CORS |
| Certificate error | `dotnet dev-certs https --trust` |
| Database missing | API creates it on first run |

## 📁 Key Files

| File | What It Does |
|------|--------------|
| TechMove.Web/Program.cs | Registers API clients & auth |
| TechMove.Web/Controllers/LoginController.cs | Handles login |
| TechMove.Web/Services/AuthenticatedHttpClientHandler.cs | Adds JWT token to requests |
| TechMove.API/Controllers/AuthController.cs | Creates JWT tokens |
| TechMove.API/Program.cs | JWT setup |

## ✅ Architecture Benefits

- ✅ MVC doesn't know about database (only API)
- ✅ API can run separately on another server
- ✅ Both layers can scale independently
- ✅ Secure: API validates all requests with JWT
- ✅ Clean: Clear separation of concerns

## 🎯 Next Steps

1. **Test everything works** - Try all CRUD operations
2. **Review documentation** - See `REFACTORING_GUIDE.md` for details
3. **For production:**
   - Change demo credentials to real user database
   - Use strong JWT secret keys
   - Restrict CORS to your domain
   - Set up proper logging & monitoring

## 📚 Full Documentation

- 📖 `REFACTORING_GUIDE.md` - Complete technical guide
- 📋 `IMPLEMENTATION_SUMMARY.md` - What changed
- 🔧 `QUICKSTART.md` - Original API setup (for reference)

## 💡 Did It Succeed?

If you can:
- ✅ Login with demo credentials
- ✅ View/create contracts via Web
- ✅ View/create clients via Web
- ✅ View/create requests via Web
- ✅ Logout successfully

**Then the refactoring is working perfectly!** 🎉

---

**Questions?** Check the troubleshooting section or review the full `REFACTORING_GUIDE.md`.
