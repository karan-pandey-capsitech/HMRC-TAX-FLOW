  
# HMRC Tax Return Management System

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
