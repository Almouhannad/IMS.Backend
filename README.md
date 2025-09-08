# IMS.Backend

## Overview
Inventory Management System (IMS) backend built with **.NET 8** and minimal APIs. It follows Clean Architecture to keep business rules independent of frameworks and focuses on maintainability and testability.

## Main Features
- Retrieve paginated products and filter by status.
- Get a single product by identifier.
- Change or sell product status with domain validation.
- Count products by status.
- Automatic database migration and CSV-based seeding.
- Structured logging with Serilog and optional Seq sink for centralized storage.


## Clean Architecture
| Layer | Project | Responsibility |
|-------|---------|---------------|
| **Domain** | `IMS.Domain` | Entities, value objects, domain errors and repository interfaces. |
| **Application** | `IMS.Application` | Use cases, commands/queries, handlers and validators implementing CQRS. |
| **Infrastructure** | `IMS.Infrastructure` | EF Core persistence, repositories, unit of work and data seeding strategies. |
| **Presentation** | `IMS.Presentation` | Minimal API endpoints mapping requests to application layer handlers. |
| **API** | `IMS.API` | Composition root that wires services and hosts the web application. |
| **Shared Kernel** | `IMS.SharedKernel` | Result type, CQRS abstractions, decorators (logging & validation) and API helpers. |
| **Config** | `IMS.Config` | Centralized environment variable access. |

### Design Patterns
- **Mediator & CQRS** – commands/queries dispatched through handlers (e.g. `SellProductCommandHandler`).
- **Result pattern** – unified success/failure handling via `Result` and `Error` types.
- **Strategy** – pluggable seeding through `IDataSeeder` with a `CSVSeeder` implementation.
- **Decorator** – logging and validation decorators wrapping handlers.
- **Repository & Unit of Work** – data access abstractions for SQL Server.

### Performance Features
- Uses EF Core `AsNoTracking` for read queries to avoid tracking overhead.
- Bulk seeding persists changes with a single `SaveChanges` call.
- Query endpoints support pagination and server-side filtering.

### Structured Logging
Logs are written in a structured form using **Serilog** and can be shipped to a
[Seq](https://datalust.co/seq) server for aggregation and search. Configure the
`SEQ_SERVER_URL` environment variable to point at your Seq instance and run it,
for example, via Docker:

```bash
docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:5341 datalust/seq
```

With the variable set, the API will emit structured logs both to the console and
to the Seq sink.

## Running with Docker Compose
1. Copy environment template and adjust as needed:
   ```bash
   cp .env.example .env
   # edit .env to set SQL_SERVER_SA_PASSWORD etc.
   ```
2. Build and start services:
   ```bash
   docker compose up --build
   ```
3. API will be available at `http://localhost:8000` (Swagger UI at `/swagger`).
4. Seq logging server will be available at `http://localhost:5341`.

## Seed Data
Startup applies migrations and seeds:
- **1500 products**
- **10 categories**

CSV seed source: `.docker/SeedData/products_seed.csv`.

## Endpoints
Base URL: `http://localhost:8000`

| Method | Endpoint | Description |
|--------|---------|-------------|
| `GET` | `/products` | List products (optional `statusFilter`, `page`, `pageSize`). |
| `GET` | `/products/{id}` | Get product by identifier. |
| `GET` | `/products/count` | Count products by status. |
| `PATCH` | `/products/{id}` | Change product status. |
| `PATCH` | `/products/{id}/sell` | Mark product as sold. |

### Examples
Retrieve paginated products:
```http
GET /products?page=1&pageSize=2 HTTP/1.1
Host: localhost:8000
```
Response:
```json
[
  {
    "id": "<guid>",
    "name": "Electronics Item 001",
    "barcode": "2000000000008",
    "description": "Electronics product 1 - generated seed data.",
    "weight": 16.02,
    "status": "InStock",
    "category": "Electronics"
  }
]
```

Change product status:
```http
PATCH /products/<guid> HTTP/1.1
Host: localhost:8000
Content-Type: application/json

{ "newStatus": "Damaged" }
```
Response: `204 No Content`

Count products by status:
```http
GET /products/count HTTP/1.1
Host: localhost:8000
```
Response:
```json
{
  "inStock": 500,
  "sold": 500,
  "damaged": 500
}
```

Mark product as sold:
```http
PATCH /products/<guid>/sell HTTP/1.1
Host: localhost:8000
```
Response: `204 No Content`

### Validation Errors
Validation failures are aggregated and returned as a single
[RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) problem details
response. Each field error includes a code, description and type. The following
request shows all parameters of `GET /products` failing validation:

```http
GET /products?statusFilter=Invalid&page=-5&pageSize=200 HTTP/1.1
Host: localhost:8000
```

Response:

```json
{
  "type": "https://httpwg.org/specs/rfc9110.html#status.422",
  "title": "Validation.General",
  "status": 422,
  "detail": "One or more validation errors occurred",
  "errors": [
    {
      "code": "PredicateValidator",
      "description": "StatusFilter must be one of the allowed product statuses (InStock, Sold, Damaged).",
      "type": 1
    },
    {
      "code": "GreaterThanValidator",
      "description": "Page must be greater than '0'.",
      "type": 1
    },
    {
      "code": "GreaterThanValidator",
      "description": "'Page Size' must be less than '20'.",
      "type": 1
    }
  ]
}
```

## Acknowledgements
Thanks to [HasanKhadd0ur](https://github.com/HasanKhadd0ur) for their contribution and helping in organizing the solution via [PR #2](https://github.com/Almouhannad/IMS.Backend/pull/2).