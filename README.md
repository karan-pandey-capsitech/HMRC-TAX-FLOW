  
# HMRC Tax Return Management System

## Current implementation status

The API includes registration, login, JWT-protected profile operations, a
health check, and an initial SA100 draft/submission workflow with client
assignment and dashboard summaries. SA800 remains planned. This is an
educational project, not a real HMRC filing system.

## Run the current API locally

Requirements: .NET 10 SDK and a reachable MongoDB instance (MongoDB Atlas is
supported). The database name defaults to `HmrcTaxFlow`. The connection string
is intentionally not stored in `appsettings.json`; configure it in User
Secrets for local development or with the `MongoDb__ConnectionString`
environment variable in a deployed environment.

From the project directory, set the connection string in User Secrets. Replace
the placeholders with your MongoDB Atlas database user, password, and cluster
host. Keep the database name in the URI and in `MongoDb:DatabaseName` aligned:

```powershell
dotnet user-secrets set "MongoDb:ConnectionString" "mongodb+srv://<db-user>:<db-password>@<cluster-host>/HmrcTaxFlow?retryWrites=true&w=majority&appName=HMRC-TAX-FLOW"
```

Atlas must allow connections from your current IP address, and the database
user must have read/write access to `HmrcTaxFlow`.

Configure a local JWT signing key in User Secrets. Do not put a real key in
`appsettings.json` or commit it:

```powershell
$bytes = New-Object byte[] 48
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($bytes)
$key = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "Jwt:Key" $key
$rng.Dispose()
```

Then start the application:

```powershell
dotnet run
```

With the Development launch profile, Swagger is available at
`http://localhost:5021/swagger` (or `https://localhost:7229/swagger` with the
HTTPS profile).

### Implemented authentication endpoints

- `POST /api/auth/register` — creates a standard `User` account. Public
  registration cannot select privileged roles.
- `POST /api/auth/login` — validates credentials and returns a JWT.
- `GET /api/auth/profile` — returns the authenticated user's profile.
- `POST /api/auth/profile/update` — updates the authenticated user's email
  and/or full name.
- `POST /api/auth/profile/delete` — deletes the authenticated user's account.
- `GET /api/health` — basic API health response.

Send the login token to protected endpoints with
`Authorization: Bearer <token>`. Public registration creates only the
standard `User` role. Once an Admin account has been provisioned through a
controlled administrative process, an Admin can assign `User`, `Practice`, or
`Debitam` with `POST /api/users/{id}/role`. There is no public or API operation
for bootstrapping an initial Admin or assigning the Admin role. Users must log
in again after a role change to receive a JWT containing the updated role.

At startup, the API creates a unique MongoDB index on usernames. Existing
databases must not contain duplicate usernames when this index is first
created.

### SA100 and dashboard endpoints

- `GET /api/clients` — lists clients visible to the signed-in user.
- `POST /api/clients` — Admin creates a client and assigns a Practice user and
  optional Debitam user. Those users must already have the corresponding role.
- `GET /api/sa100` and `GET /api/sa100/{id}` — Admin sees all returns; Practice
  sees returns for their assigned clients.
- `POST /api/sa100` — Practice creates a draft for an assigned client.
- `POST /api/sa100/{id}` — Practice updates an owned draft.
- `POST /api/sa100/{id}/submit` — Practice submits an owned draft. Submitted
  returns cannot be edited or submitted again.
- `GET /api/dashboard` — Admin sees system-wide SA100 counts; Practice sees
  counts for their own returns.

The SA100 tax-year input uses `YYYY-YY` (for example, `2025-26`). Monetary
amounts must be non-negative. Estimated Tax is stored as entered; no tax
calculation formula is defined by this educational project. Role assignment
and initial Admin provisioning remain controlled administrative operations;
public registration cannot assign privileged roles.

## 1. Project Goal
Build a small ASP.NET Core Web API + MongoDB application for managing simplified HMRC tax returns.

The system will support:
- SA100 - Individual Self Assessment
- SA800 - Partnership Tax Return
- JWT Login
- Role-based authorization
- MongoDB
- Modular Monolith Architecture

This is an educational project, not a real HMRC filing system.

## 2. Technology
- C#
- ASP.NET Core Web API
- MongoDB
- MongoDB.Driver
- JWT Authentication
- Swagger
- Modular Monolith

## 3. User Roles

### Admin
- Manage users
- Assign roles
- View all returns
- View dashboard

### Practice
Practice represents the accountant/tax practice user.
- Create/edit SA100
- Create/edit SA800
- Save Draft
- Submit returns
- View assigned clients

### Debitam
Debitam represents the bookkeeping/financial-data user.
- Add income
- Add expenses
- Maintain financial information
- View assigned clients
- Cannot manage users or submit returns

## 4. SA100

SA100 is the individual Self Assessment return.

Keep only:
- Tax Year
- Client Name
- National Insurance Number
- Employment Income
- Self Employment Income
- Other Income
- Tax Already Paid
- Estimated Tax
- Status

Flow:

Login → Practice → Client → SA100 → Save Draft → Submit

## 5. SA800

SA800 is the Partnership Tax Return.

Keep only:
- Tax Year
- Partnership Name
- UTR
- Total Income
- Total Expenses
- Profit
- Partners
- Partner Percentage
- Allocated Profit
- Status

Calculation:

Profit = Income - Expenses

Partner Profit = Profit × Partner Percentage / 100

Example:

Income = £100,000
Expenses = £40,000
Profit = £60,000

Partner A = 60% → £36,000
Partner B = 40% → £24,000

## 6. MongoDB

Use only three main collections:
```

users sa100_returns sa800_returns

```

User:
```

Id Name Email PasswordHash Role IsActive

```

SA100 and SA800 should contain the fields defined above.

## 7. Authentication

Use JWT.
```

POST /api/auth/login

```

Flow:
```

Login ↓ Validate User ↓ Generate JWT ↓ Return Token + Role ↓ Access Protected APIs

```

Use:
```

\[Authorize(Roles = "Admin")\]

```

or:
```

\[Authorize(Roles = "Admin,Practice")\]

```

## 8. APIs

Use GET and POST as the default project convention.

### Auth
```

POST /api/auth/login

```

### Users
```

GET /api/users POST /api/users POST /api/users/{id}/deactivate

```

### SA100
```

GET /api/sa100 GET /api/sa100/{id} POST /api/sa100 POST /api/sa100/{id} POST /api/sa100/{id}/submit

```

### SA800
```

GET /api/sa800 GET /api/sa800/{id} POST /api/sa800 POST /api/sa800/{id} POST /api/sa800/{id}/submit

```

Do not use PUT or DELETE unless absolutely necessary.

## 9. Architecture

Use a Modular Monolith.
```

HMRC.TaxSystem │ ├── HMRC.Api │ └── Controllers │ ├── HMRC.Application │ ├── Authentication │ ├── Users │ ├── SA100 │ └── SA800 │ ├── HMRC.Domain │ ├── Users │ ├── SA100 │ └── SA800 │ └── HMRC.Infrastructure ├── MongoDB ├── Repositories └── Authentication

```

One application + one MongoDB database. Do NOT create microservices.

## 10. Development Order

Complete the project in this order:
```

1. Create ASP.NET Core Web API
2. Create Domain/Application/Infrastructure projects
3. Configure MongoDB
4. Create User + JWT Login
5. Add Admin/Practice/Debitam roles
6. Create SA100 APIs + basic calculation
7. Create SA800 APIs + profit calculation
8. Add Draft/Submitted status
9. Add role authorization
10. Test everything using Swagger

```

### Final Workflow
```

LOGIN ↓ ROLE ↓ DASHBOARD ↓ SA100 / SA800 ↓ ENTER DATA ↓ CALCULATE ↓ DRAFT ↓ SUBMIT ↓ MONGODB

```

 
```
