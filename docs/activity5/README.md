# Activity 5 setup and evidence

The API hosts application workflows and EF Core/Npgsql persistence. The desktop consumes JSON over asynchronous HTTP. Existing migrations are preserved; no schema change is necessary for this boundary change.

## Start the server

From the repository root in PowerShell, set the connection string locally (never commit the real password):

```powershell
$env:EquipmentBorrowing__ConnectionString = 'Host=localhost;Port=5432;Database=equipment_borrowing;Username=postgres;Password=YOUR_PASSWORD'
dotnet build EquipmentBorrowing.slnx
dotnet run --project src/EquipmentBorrowing.Api -- --initialize
dotnet run --project src/EquipmentBorrowing.Api
```

`--initialize` applies the existing migrations and inserts samples only into empty student/equipment tables, then exits. The normal HTTP host starts without connecting to PostgreSQL, so a database outage is represented by a safe HTTP 500 on a data route.

API base: `http://localhost:5080/api/v1/`. Machine-readable contract: `http://localhost:5080/openapi/v1.json`; captured copy: [openapi.json](openapi.json).

## Start the desktop separately

In another PowerShell window:

```powershell
$env:EquipmentBorrowing__ApiBaseUrl = 'http://localhost:5080/api/v1/'
dotnet run --project src/EquipmentBorrowing.Desktop
```

The desktop does not need a PostgreSQL password. Its Infrastructure and EF Core references have been removed. HTTP services are in `src/EquipmentBorrowing.Desktop/Services/ApiClient.cs`; server endpoints are in `src/EquipmentBorrowing.Api/CampusController.cs`. Server application coordination is in `ApiBorrowingService`, student/equipment management services, and the lookup service. Repositories use short-lived contexts and database transactions.

## Disposable acceptance database

Create `equipment_borrowing_lab5_acceptance` in pgAdmin. Set the connection string to that database in the API terminal. Never run failure demonstrations against the coursework database.

```powershell
dotnet run --project src/EquipmentBorrowing.Api -- --reset-acceptance
dotnet run --project src/EquipmentBorrowing.Api
```

Reset deletes and recreates data only when the database name ends in `_lab5_acceptance`. It retains schema and migrations. It seeds S1–S4 and E1–E7 from the instructor reference. IDs are resolved from API responses. Dates use the server's local calendar: E1 was borrowed nine days ago and is due yesterday, E3 was borrowed eight days ago and returned two days ago, E4/E5 were borrowed seven days ago. Other due dates are two days later. Record actual execution date rather than sending the expired November 12 sample date.

In a PowerShell 7 terminal with the same acceptance connection string, set `PGPASSWORD` locally for the independent psql reader, then run:

```powershell
./scripts/Invoke-Lab5Acceptance.ps1
```

The script expects PostgreSQL 19 psql in its standard Windows installation directory. Evidence contains JSON requests, responses, status codes and fresh PostgreSQL snapshots. The harness does not replace the UI walkthrough or every semantic assertion.

To demonstrate known rollback, start an API process with `EquipmentBorrowing__AcceptanceFault=after-save` and the disposable database. Checkout throws after SaveChanges and before commit; a fresh database snapshot must equal the pre-request snapshot. Remove the setting after the demonstration. The fault is ignored for other database names.

## Contract and recovery

The /api/v1 routes and DTOs implement the sixteen reference contracts. Creation responses use 201 and Location where specified. Lists return arrays; histories of existing subjects can be empty. Invalid input is 400, missing records 404, conflicts 409, unexpected database failures 500. Errors have code and message fields. Status is Active/Returned and dates are YYYY-MM-DD. Unknown request fields are rejected.

An HTTP connection failure has no response status. HTTP 500 means a response was received from the server. On a mutation timeout or lost connection, the desktop asks the user to refresh before retrying; it never automatically repeats mutations. An uncertain database commit uses outcome_unknown. API logs record exception type and error code without exception stack traces or connection strings in client responses.

## Submission status

See [baseline](../activity5-baseline.md), [execution record](execution.md), and [traceability](traceability.md). Add actual UI evidence before marking the entire acceptance suite complete. Create `lab5-submitted` only on the final completed revision; no historical submission tag is fabricated.
