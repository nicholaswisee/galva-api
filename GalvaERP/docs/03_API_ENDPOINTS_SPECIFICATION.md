# 03. REST API Endpoint Specification

> **Audience:** Frontend Engineers, API Integrators, and AI Agents.
> Complete specification for all REST endpoints in GalvaERP API (`galva-api`).

---

## 1. Authentication & Security Endpoints (`/api/auth`)

### 1.1 POST `/api/auth/register`
- **Purpose:** Registers a new user account.
- **Request Body:**
  ```json
  {
    "username": "john.doe",
    "password": "SecretPassword123!",
    "fullName": "John Doe",
    "email": "john.doe@galva.co.id",
    "role": "Purchasing"
  }
  ```
- **Response `201 Created`:**
  ```json
  {
    "userId": "usr-001",
    "username": "john.doe",
    "message": "User registered successfully."
  }
  ```

### 1.2 POST `/api/auth/login`
- **Purpose:** Authenticates user credentials and issues a JWT token.
- **Request Body:**
  ```json
  {
    "username": "john.doe",
    "password": "SecretPassword123!"
  }
  ```
- **Response `200 OK`:**
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-07-30T18:00:00Z",
    "user": {
      "username": "john.doe",
      "fullName": "John Doe",
      "role": "Purchasing"
    }
  }
  ```

---

## 2. Master Data Endpoints (`/api/masterdata`)

- `GET /api/masterdata/suppliers` — Returns active vendor directory (`Supplier`).
- `GET /api/masterdata/departments` — Returns department directory (`Dept`).
- `GET /api/masterdata/warehouses` — Returns warehouse list (`Gudang`).
- `GET /api/masterdata/banks` — Returns company bank accounts (`Bank`).
- `GET /api/masterdata/items` — Returns product item catalog (`Barang`).

---

## 3. Purchase Requisition Endpoints (`/api/purchase-requisitions`)

### 3.1 GET `/api/purchase-requisitions`
- **Response `200 OK`:** Array of `PRListDto`.

### 3.2 GET `/api/purchase-requisitions/{*doku}`
- **URL Parameter:** `doku` (Catch-all, URL-decoded via `RouteParams.Decode`).
- **Response Header:** `ETag: "Base64RowVersion=="`
- **Response `200 OK`:** `PRDetailDto` object.

### 3.3 POST `/api/purchase-requisitions`
- **Headers:** `Idempotency-Key` (Optional).
- **Request Body:**
  ```json
  {
    "tgl": "2026-07-30T00:00:00Z",
    "kode_Dept": "DEPT-01",
    "kode_Sales": "SLS-01",
    "memo": "Monthly office supplies request",
    "lineItems": [
      {
        "kode_Brg": "BRG-1001",
        "jumlah": 10.0,
        "harga": 150000.0,
        "kode_Gudang": "GDG-01",
        "alias": "Paper A4"
      }
    ]
  }
  ```
- **Response `201 Created`:** `{ "doku": "SPB-20260730-001" }`

### 3.4 PUT `/api/purchase-requisitions/{*doku}`
- **Headers:** `If-Match: "Base64RowVersion=="` (Required).
- **Response `200 OK`:** Updated `PRDetailDto`. `ETag` header refreshed.

---

## 4. Purchase Order Endpoints (`/api/purchase-orders`)

> Note: `/api/purchase-orders` handles Purchase Orders (Draft `POSem` & Confirmed `PO`).

### 4.1 GET `/api/purchase-orders`
- **Response `200 OK`:** Array of `POListDto`.

### 4.2 GET `/api/purchase-orders/{*doku}`
- **Response Header:** `ETag: "Base64RowVersion=="`
- **Response `200 OK`:** `PODetailDto` payload.

### 4.3 POST `/api/purchase-orders`
- **Request Body:**
  ```json
  {
    "tgl": "2026-07-30T00:00:00Z",
    "kode_Supplier": "SUPP-001",
    "kode_dept": "DEPT-01",
    "kode_Valas": "IDR",
    "kurs": 1.0,
    "syarat": 30,
    "ppn": 11.0,
    "diskon": 0.0,
    "memo": "PO for office furniture",
    "lineItems": [
      {
        "kode_Brg": "BRG-2001",
        "merk": "Ergonomic",
        "model": "Chair-X",
        "satuan": "PCS",
        "jumlah": 5.0,
        "harga": 1200000.0,
        "discPct": 0.0,
        "disc": 0.0,
        "kode_Gudang": "GDG-01"
      }
    ]
  }
  ```
- **Response `201 Created`:** `{ "doku": "PO-20260730-001" }`

### 4.4 PUT `/api/purchase-orders/{*doku}`
- **Headers:** `If-Match: "Base64RowVersion=="` (Required).
- **Response `200 OK`:** Updated `PODetailDto`.

### 4.5 DELETE `/api/purchase-orders/{*doku}`
- **Headers:** `If-Match: "Base64RowVersion=="` (Required).
- **Constraints:** Returns `409 Conflict` if `STS != "0"`.
- **Response `204 No Content`:** Soft-deleted (`Hapus = Username`).

---

## 5. Goods Receipt Endpoints (`/api/goods-receipts`)

- `GET /api/goods-receipts` — List all active receipts.
- `GET /api/goods-receipts/{*doku}` — Get detail of receipt by document number.
- `POST /api/goods-receipts` — Create goods receipt against confirmed `PO`.
- `PUT /api/goods-receipts/{*doku}` — Update receipt header (requires `If-Match`).

---

## 6. AP Invoice Endpoints (`/api/invoices`)

- `GET /api/invoices` — List AP vouchers.
- `GET /api/invoices/{*doku}` — Get AP invoice detail.
- `POST /api/invoices` — Post AP invoice linked to `LPB` or `PO`.
- `PUT /api/invoices/{*doku}` — Update invoice (requires `If-Match`).

---

## 7. Payment Endpoints (`/api/payments`)

- `GET /api/payments` — List vendor payment vouchers.
- `GET /api/payments/{*doku}` — Get payment voucher detail.
- `POST /api/payments` — Disburse payment for AP invoices.

---

## 8. HTTP Status Codes Reference

| Code | Status Name | Usage in API |
|---|---|---|
| `200` | OK | Successful GET, PUT operations |
| `201` | Created | Successful POST creation |
| `204` | No Content | Successful DELETE soft-delete |
| `400` | Bad Request | Validation failure, malformed JSON, invalid payload |
| `401` | Unauthorized | Missing or invalid JWT Bearer token |
| `404` | Not Found | Target document number does not exist or is soft-deleted |
| `409` | Conflict | Business rule violation or Idempotency processing collision |
| `412` | Precondition Failed | Concurrency mismatch (`If-Match` ETag mismatch) |
| `428` | Precondition Required | Missing required `If-Match` header on update/delete |
