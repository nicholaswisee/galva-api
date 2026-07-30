# 04. Agentic Development & Playbook Guide

> **For Autonomous Agents & Developers.** This document is the primary operational guide for creating, extending, or maintaining features in `galva-api`.

---

## 1. Principles of Development

1. **Strict Vertical Slices:** Every feature stage resides in its own isolated folder under `Features/<FeatureName>/` with dedicated Commands, Queries, DTOs, Validators, and Endpoints.
2. **MediatR CQRS Pipeline:** Controllers/Endpoints MUST NOT contain business logic. Endpoints only parse request headers/routes and delegate execution to `IMediator.Send()`.
3. **Database Single Source of Truth:** `galva-db/schema.sql` is the sole source of truth for the database schema. Never infer column names or types without reading `schema.sql` and `01_DATABASE_MAPPING_GUIDE.md`.
4. **Empirical Verification:** Never declare success on a code change without compiling (`dotnet build`) and verifying clean execution.

---

## 2. Step-by-Step Feature Slice Creation Playbook

When adding a new endpoint or feature slice, follow this exact workflow:

### Step 1: Define DTO Contracts
Create immutable `record` DTOs under `Features/<FeatureName>/DTOs/`:
```csharp
namespace GalvaERP.Features.SampleFeature.DTOs;

public record SampleDetailDto(
    string Doku,
    DateTime Tgl,
    string Status,
    double Total,
    string ETag);
```

### Step 2: Create MediatR Command / Query
Create the request object under `Features/<FeatureName>/Commands/` or `Queries/`:
```csharp
namespace GalvaERP.Features.SampleFeature.Commands;

public record CreateSampleCommand(
    DateTime Tgl,
    string Kode_Dept,
    List<SampleLineDto> LineItems) : IRequest<string>;
```

### Step 3: Implement FluentValidation Rules
Add validation rules in `Features/<FeatureName>/Commands/<CommandName>Validator.cs`:
```csharp
public class CreateSampleCommandValidator : AbstractValidator<CreateSampleCommand>
{
    public CreateSampleCommandValidator()
    {
        RuleFor(x => x.Kode_Dept).NotEmpty().WithMessage("Kode_Dept is required.");
        RuleFor(x => x.LineItems).NotEmpty().WithMessage("At least one line item required.");
    }
}
```

### Step 4: Implement Request Handler
Write the CQRS handler in `Features/<FeatureName>/Commands/<CommandName>Handler.cs`:
- Auto-generate document number using standard prefix (`SMP-yyyyMMdd-XXX`).
- Apply financial calculation formulas from `02_P2P_BUSINESS_LOGIC_GUIDE.md`.
- Set `EntryDate = DateTime.Now`.
- Save changes via `_context.SaveChangesAsync(cancellationToken)`.

### Step 5: Register Minimal API Endpoint Map
Expose the endpoint in `Features/<FeatureName>/SampleFeatureEndpoints.cs`:
- Map route group: `app.MapGroup("/api/sample").WithTags("Sample")`.
- Implement `ETag` and `If-Match` handling for mutating routes (`PUT`, `DELETE`).
- Register route map in `Program.cs`.

---

## 3. Strict Rules & Agentic Gotchas

### 🚨 Rule 1: Never Insert or Update Computed Columns
`SubSPB.Jumhar` is a SQL Server computed column: `[Jumhar] AS (([jumlah]*[Harga]))`.
- **Failure Case:** Attempting to assign `subSpb.Jumhar = ...` in code will cause an EF Core / SQL Server exception (`Cannot insert explicit value into computed column`).
- **Fix:** Do not assign `Jumhar` in C# code. Let EF Core ignore it or mark it `.ValueGeneratedOnAddOrUpdate()`.

### 🚨 Rule 2: Always Handle `If-Match` Headers for Mutations
Mutating endpoints (`PUT`, `DELETE`) MUST require an `If-Match` header containing the Base64 ETag of the `RowVersion`.
- Parse ETag using `Convert.FromBase64String(raw)`.
- Pass `IfMatchRowVersion` to handler.
- Attach `OriginalValue` in handler:
  ```csharp
  _context.Entry(entity).Property(e => e.RowVersion).OriginalValue = request.IfMatchRowVersion;
  ```
- Catch `DbUpdateConcurrencyException` and throw `ConcurrencyException` to return HTTP `412 Precondition Failed`.

### 🚨 Rule 3: Soft Delete Enforcement
Never execute physical `_context.Entities.Remove(entity)` on ERP transaction tables.
- Populate `entity.Hapus = $"{username}|{DateTime.Now:s}"`.
- Ensure all query slices filter `WHERE Hapus IS NULL`.

### 🚨 Rule 4: Catch-All Route Parameters & Slashes in `Doku`
Document numbers like `SPB-20260730-001` or `SPB/2026/07/001` contain slashes and special characters.
- Use catch-all route syntax: `app.MapGet("/{*doku}", ...)`
- Always decode parameter: `var decodedDoku = RouteParams.Decode(doku);`

### 🚨 Rule 5: No Hardcoded Database Schema Assumptions
Always verify entity property names against `Domain/Entities/` and `schema.sql`.
- Note column spelling variations across tables:
  - `Kode_dept` in `PO` vs `Kode_Dept` in `SPB`
  - `PPN` in `PO` vs `PPn` in `SPB`
  - `Sts` in `SPB` vs `STS` in `PO` and `LPB`
