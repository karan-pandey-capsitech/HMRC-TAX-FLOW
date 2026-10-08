# SA100 API: Copy-ready test guide

Use these endpoints in order to test the complete SA100 workflow. The API uses
JSON and only `GET` and `POST` methods for the operations described here.

## Before you start

- Start the API and MongoDB.
- Use an existing account with the `Practice` role. Public registration creates
  a standard `User` role, which cannot create or submit SA100 returns. A
  Practice account and its client assignment must be provisioned by an admin.
- The examples use `http://localhost:5021` as the API base URL. Replace it with
  the URL shown by your application if it differs.
- Replace values such as `YOUR_JWT`, `CLIENT_ID`, and `RETURN_ID` with values
  from your API responses. IDs must be valid GUIDs.

All protected endpoints require this header:

```http
Authorization: Bearer YOUR_JWT
```

## 1. Log in

**POST** `/api/auth/login` — public

```http
POST http://localhost:5021/api/auth/login
Content-Type: application/json
```

```json
{
  "username": "",
  "password": ""
}
```

Example response:

```json
{
  "token": "eyJ...",
  "expiresAt": "2026-10-08T13:00:00Z"
}
```

Copy the `token` value. Use it as `YOUR_JWT` in all following requests.

## 2. Register the account (if it does not exist)

**POST** `/api/auth/register` — public

```http
POST http://localhost:5021/api/auth/register
Content-Type: application/json
```

```json
{
  "username": "practice.user",
  "email": "practice@example.com",
  "password": "ChangeThisPassword123!",
  "fullName": "Practice User"
}
```

Registration creates a `User` account. It does not grant Practice privileges.

## 3. Assign the Practice role

Log in as Admin using `POST /api/auth/login` from step 1. Then assign the
registered account the Practice role:

**POST** `/api/users/{id}/role` — Admin only

```http
POST http://localhost:5021/api/users/PRACTICE_USER_ID/role
Authorization: Bearer ADMIN_JWT
Content-Type: application/json
```

```json
{
  "role": "Practice"
}
```

Replace `PRACTICE_USER_ID` with the account's ID. The ID is returned by
registration, or by `GET /api/auth/profile` when logged in as that account.
This endpoint replaces the account's role with the requested role. It accepts
`User`, `Practice`, or `Debitam`; Admin assignment remains a controlled
provisioning operation. Log out and log back in as the Practice user after the
role change because the old JWT still contains the previous role.

## 4. Create a client and assign it to the Practice user

**POST** `/api/clients` — Admin only

```http
POST http://localhost:5021/api/clients
Authorization: Bearer ADMIN_JWT
Content-Type: application/json
```

```json
{
  "name": "Example Client",
  "nationalInsuranceNumber": "AB123456C",
  "practiceUserId": "PRACTICE_USER_ID",
  "debitamUserId": null
}
```

Replace `PRACTICE_USER_ID` with the registered user's ID after assigning the
Practice role. `debitamUserId` can be `null` or the ID of an existing Debitam
user. The API returns the created client and its ID.

## 5. List clients assigned to you

**GET** `/api/clients` — Admin, Practice, or Debitam

```http
GET http://localhost:5021/api/clients
Authorization: Bearer YOUR_JWT
```

Example response:

```json
[
  {
    "id": "8b32fa7d-6b58-4a6e-9fc9-05295118282f",
    "name": "Example Client",
    "nationalInsuranceNumber": "AB123456C",
    "practiceUserId": "47d40c8c-9b88-42dd-8eb9-11c3a9706732",
    "debitamUserId": null,
    "createdAt": "2026-10-08T10:00:00Z"
  }
]
```

Copy the assigned client's `id` as `CLIENT_ID`. The client name and National
Insurance number on the SA100 are loaded from this client record.

## 6. Create a draft

**POST** `/api/sa100` — Practice

```http
POST http://localhost:5021/api/sa100
Authorization: Bearer YOUR_JWT
Content-Type: application/json
```

```json
{
  "clientId": "CLIENT_ID",
  "taxYear": "2025-26",
  "employmentIncome": 42000.00,
  "selfEmploymentIncome": 0.00,
  "otherIncome": 1200.00,
  "taxAlreadyPaid": 8500.00,
  "estimatedTax": 9100.00
}
```

Replace `CLIENT_ID` with the GUID returned by `GET /api/clients` (without the
placeholder text). Tax year uses `YYYY-YY`; monetary values must be from 0 to
999,999,999.99. A client can have only one return per tax year.

Expected status: **201 Created**. Example response:

```json
{
  "id": "f75aceb8-9aae-44ed-a3c6-59251b3d82c2",
  "clientId": "8b32fa7d-6b58-4a6e-9fc9-05295118282f",
  "taxYear": "2025-26",
  "clientName": "Example Client",
  "nationalInsuranceNumber": "AB123456C",
  "employmentIncome": 42000.00,
  "selfEmploymentIncome": 0.00,
  "otherIncome": 1200.00,
  "taxAlreadyPaid": 8500.00,
  "estimatedTax": 9100.00,
  "status": 0,
  "createdBy": "47d40c8c-9b88-42dd-8eb9-11c3a9706732",
  "createdAt": "2026-10-08T10:30:00Z",
  "updatedAt": "2026-10-08T10:30:00Z",
  "submittedBy": null,
  "submittedAt": null
}
```

Save the returned `id` as `RETURN_ID`. In the current API, `status` is an enum
serialized as a number: `0` means Draft and `1` means Submitted.

## 7. Read the saved return

**GET** `/api/sa100` — Admin or Practice

```http
GET http://localhost:5021/api/sa100
Authorization: Bearer YOUR_JWT
```

Admin receives all returns. Practice receives returns belonging to their
assigned clients.

**GET** `/api/sa100/{id}` — Admin or Practice

```http
GET http://localhost:5021/api/sa100/RETURN_ID
Authorization: Bearer YOUR_JWT
```

Replace `RETURN_ID` with the ID returned by the create request.

## 8. Update the draft

**POST** `/api/sa100/{id}` — Practice

```http
POST http://localhost:5021/api/sa100/RETURN_ID
Authorization: Bearer YOUR_JWT
Content-Type: application/json
```

```json
{
  "employmentIncome": 43000.00,
  "selfEmploymentIncome": 0.00,
  "otherIncome": 1200.00,
  "taxAlreadyPaid": 8700.00,
  "estimatedTax": 9300.00
}
```

Send the complete set of editable amount fields. Omitted decimal fields bind as
zero. Client identity and tax year cannot be changed. Only the owner of a Draft
can update it.

Expected status: **200 OK** with the updated return.

## 9. Submit the draft

**POST** `/api/sa100/{id}/submit` — Practice

```http
POST http://localhost:5021/api/sa100/RETURN_ID/submit
Authorization: Bearer YOUR_JWT
```

No request body is required. Expected status: **200 OK** with `status: 1`,
`submittedBy`, and `submittedAt` set. A submitted return cannot be edited or
submitted again. Debitam users are not allowed to submit.

## 10. Check dashboard totals

**GET** `/api/dashboard` — Admin or Practice

```http
GET http://localhost:5021/api/dashboard
Authorization: Bearer YOUR_JWT
```

Example response after submitting the single test return:

```json
{
  "totalReturns": 1,
  "draftReturns": 0,
  "submittedReturns": 1
}
```

Admin sees system-wide counts. Practice sees counts for their own returns.
The dashboard reads counts from saved SA100 records; it does not keep a second
copy of the data.

## Access and error responses

| HTTP status | Meaning |
|---|---|
| `400 Bad Request` | Invalid JSON or request validation failed. |
| `401 Unauthorized` | JWT is missing, invalid, or expired. |
| `403 Forbidden` | The user's role or client/return assignment does not permit the operation. |
| `404 Not Found` | The client or return ID does not exist. |
| `409 Conflict` | A return already exists for that client and tax year, or the return is no longer a Draft. |

Roles: Admin and Practice can read SA100 returns; only Practice can create,
update, and submit. Admin is not permitted to submit through these endpoints.
