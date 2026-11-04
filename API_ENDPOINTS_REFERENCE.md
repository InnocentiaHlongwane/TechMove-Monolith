# TechMove API - Endpoint Reference

## 🔍 Contracts API

### Base URL: `/api/contracts`

| Method | Endpoint | Description | Parameters | Auth |
|--------|----------|-------------|-----------|------|
| **GET** | `/` | List all contracts | `startDate?`, `endDate?`, `status?` | - |
| **GET** | `/{id}` | Get contract details | `id` (path) | - |
| **POST** | `/` | Create new contract | Form data: clientId, startDate, endDate, serviceLevel, signedAgreement (file) | - |
| **PATCH** | `/{id}/status` | Update status | `id` (path), Body: `{ status: string }` | - |
| **GET** | `/{id}/download` | Download PDF agreement | `id` (path) | - |

### Request/Response Examples

#### GET /api/contracts
```
Request:
GET /api/contracts?startDate=2024-01-01&endDate=2025-01-01&status=Active

Response (200 OK):
[
  {
	"id": 1,
	"clientId": 1,
	"clientName": "Lebo",
	"startDate": "2024-01-15T00:00:00Z",
	"endDate": "2025-01-15T00:00:00Z",
	"status": "Active",
	"serviceLevel": "Premium",
	"signedAgreementPath": "uploads/contracts/abc123.pdf"
  }
]
```

#### POST /api/contracts
```
Request:
POST /api/contracts
Content-Type: multipart/form-data

Form Fields:
- clientId: 1
- startDate: 2024-02-01
- endDate: 2025-02-01
- serviceLevel: Standard
- signedAgreement: [PDF file]

Response (201 Created):
{
  "id": 2,
  "clientId": 1,
  "clientName": "Lebo",
  "startDate": "2024-02-01T00:00:00Z",
  "endDate": "2025-02-01T00:00:00Z",
  "status": "Draft",
  "serviceLevel": "Standard",
  "signedAgreementPath": "uploads/contracts/xyz789.pdf"
}
```

#### PATCH /api/contracts/{id}/status
```
Request:
PATCH /api/contracts/1/status
Content-Type: application/json

{
  "status": "Active"
}

Response (200 OK):
{
  "id": 1,
  "clientId": 1,
  "clientName": "Lebo",
  "startDate": "2024-01-15T00:00:00Z",
  "endDate": "2025-01-15T00:00:00Z",
  "status": "Active",
  "serviceLevel": "Premium",
  "signedAgreementPath": "uploads/contracts/abc123.pdf"
}
```

---

## 👥 Clients API

### Base URL: `/api/clients`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|-----------|
| **GET** | `/` | List all clients | - |
| **GET** | `/{id}` | Get client details | `id` (path) |
| **POST** | `/` | Create new client | Body: `{ name, contactDetails, region }` |
| **PUT** | `/{id}` | Update client | `id` (path), Body: `{ name, contactDetails, region }` |
| **DELETE** | `/{id}` | Delete client | `id` (path) |

### Request/Response Examples

#### GET /api/clients
```
Response (200 OK):
[
  {
	"id": 1,
	"name": "Lebo",
	"contactDetails": "lebo@example.com",
	"region": "South Africa"
  },
  {
	"id": 2,
	"name": "John Doe",
	"contactDetails": "john@example.com",
	"region": "Gauteng"
  }
]
```

#### POST /api/clients
```
Request:
POST /api/clients
Content-Type: application/json

{
  "name": "New Client",
  "contactDetails": "contact@newclient.com",
  "region": "Western Cape"
}

Response (201 Created):
{
  "id": 3,
  "name": "New Client",
  "contactDetails": "contact@newclient.com",
  "region": "Western Cape"
}
```

---

## 📋 Service Requests API

### Base URL: `/api/servicerequests`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|-----------|
| **GET** | `/` | List all requests | - |
| **GET** | `/contract/{contractId}` | Get requests by contract | `contractId` (path) |
| **GET** | `/{id}` | Get request details | `id` (path) |
| **POST** | `/` | Create new request | Body: `{ contractId, description, cost, currency? }` |
| **PUT** | `/{id}` | Update request | `id` (path), Body: `{ description, cost, currency? }` |
| **DELETE** | `/{id}` | Delete request | `id` (path) |

### Request/Response Examples

#### GET /api/servicerequests
```
Response (200 OK):
[
  {
	"id": 1,
	"contractId": 1,
	"description": "Website maintenance",
	"cost": 5000.00,
	"currency": "ZAR",
	"createdAt": "2024-01-20T10:30:00Z"
  }
]
```

#### POST /api/servicerequests
```
Request:
POST /api/servicerequests
Content-Type: application/json

{
  "contractId": 1,
  "description": "Database optimization",
  "cost": 8500.00,
  "currency": "ZAR"
}

Response (201 Created):
{
  "id": 2,
  "contractId": 1,
  "description": "Database optimization",
  "cost": 8500.00,
  "currency": "ZAR",
  "createdAt": "2024-01-25T14:15:00Z"
}
```

#### GET /api/servicerequests/contract/1
```
Response (200 OK):
[
  {
	"id": 1,
	"contractId": 1,
	"description": "Website maintenance",
	"cost": 5000.00,
	"currency": "ZAR",
	"createdAt": "2024-01-20T10:30:00Z"
  },
  {
	"id": 2,
	"contractId": 1,
	"description": "Database optimization",
	"cost": 8500.00,
	"currency": "ZAR",
	"createdAt": "2024-01-25T14:15:00Z"
  }
]
```

---

## 🔄 Contract Status Transitions

Valid status transitions:
- **Draft** → Active, Expired, OnHold
- **Active** → Expired, OnHold
- **Expired** → (terminal state)
- **OnHold** → Active

Example: You can move a contract from Draft to Active, but not from Expired back to Active.

---

## 🎛️ Query Parameters

### Contracts Filtering
```
GET /api/contracts?startDate=2024-01-01&endDate=2024-12-31&status=Active

Parameters:
- startDate: ISO 8601 date (optional)
  Example: 2024-01-01, 2024-01-01T00:00:00Z

- endDate: ISO 8601 date (optional)
  Example: 2024-12-31, 2024-12-31T23:59:59Z

- status: Contract status (optional)
  Values: Draft, Active, Expired, OnHold
```

---

## ❌ Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
	"fieldName": ["Error message here"]
  }
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404
}
```

### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "traceId": "00-abc123-def456-00"
}
```

---

## 📤 File Upload (Contracts)

### Upload Signed Agreement PDF

```
POST /api/contracts
Content-Type: multipart/form-data

Form-data:
- clientId: 1 (integer)
- startDate: 2024-02-01 (date, YYYY-MM-DD)
- endDate: 2025-02-01 (date, YYYY-MM-DD)
- serviceLevel: Premium (string)
- signedAgreement: [binary PDF file content] (file)

Validations:
- PDF only (checked by filename and content-type)
- Required: cannot be empty
- File size: reasonable limits enforced
- Client must exist
- endDate >= startDate
```

### Download Signed Agreement

```
GET /api/contracts/1/download

Response: PDF file (application/pdf)
Header: Content-Disposition: attachment; filename="Contract-1-Agreement.pdf"
```

---

## 🗂️ Data Models

### Contract
```json
{
  "id": 1,
  "clientId": 1,
  "clientName": "Client Name",
  "startDate": "2024-01-15T00:00:00Z",
  "endDate": "2025-01-15T00:00:00Z",
  "status": "Active",
  "serviceLevel": "Premium",
  "signedAgreementPath": "uploads/contracts/filename.pdf"
}
```

### Client
```json
{
  "id": 1,
  "name": "Client Name",
  "contactDetails": "contact@example.com",
  "region": "South Africa"
}
```

### ServiceRequest
```json
{
  "id": 1,
  "contractId": 1,
  "description": "Service description",
  "cost": 5000.00,
  "currency": "ZAR",
  "createdAt": "2024-01-20T10:30:00Z"
}
```

---

## 🧪 Testing with Swagger

1. Navigate to `https://localhost:56373/swagger/ui`
2. Click on any endpoint to expand it
3. Click **"Try it out"**
4. Fill in the parameters
5. Click **"Execute"**
6. View the response

---

## 📊 HTTP Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 | OK | Successful GET or PUT request |
| 201 | Created | Successful POST request |
| 204 | No Content | Successful DELETE request |
| 400 | Bad Request | Invalid input or validation failure |
| 404 | Not Found | Resource doesn't exist |
| 500 | Server Error | Internal server error |

---

**API Version**: v1.0
**Base URL**: `https://localhost:56373/api`
**Documentation**: Available at `/swagger/ui`
**OpenAPI Spec**: Available at `/swagger/v1/swagger.json`
