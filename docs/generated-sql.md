# LINQ Queries and Generated SQL

EF Core translates the repository LINQ queries to SQLite SQL. The SQL below is captured from `ToQueryString()` for the configured EF Core SQLite provider. Alias and selected-column ordering can vary between EF Core versions.

## Available equipment

LINQ:

```csharp
dbContext.Equipment
    .AsNoTracking()
    .Where(item => item.IsAvailable)
    .OrderBy(item => item.Name)
    .ToListAsync(cancellationToken);
```

Generated SQL:

```sql
SELECT "e"."Id", "e"."IsAvailable", "e"."Name"
FROM "Equipment" AS "e"
WHERE "e"."IsAvailable"
ORDER BY "e"."Name"
```

Explanation: this reads available equipment, sorts by name, and does not track the returned entities because the screen only displays them.

## Active borrowings with related information

LINQ:

```csharp
dbContext.Borrowings
    .AsNoTracking()
    .Include(borrowing => borrowing.Student)
    .Include(borrowing => borrowing.Equipment)
    .Where(borrowing => borrowing.Status == BorrowingStatus.Active)
    .OrderBy(borrowing => borrowing.ExpectedReturnDate)
    .ToListAsync(cancellationToken);
```

Generated SQL:

```sql
SELECT "b"."Id", "b"."DateBorrowed", "b"."EquipmentId",
       "b"."ExpectedReturnDate", "b"."Status", "b"."StudentId",
       "s"."Id", "s"."IsAllowedToBorrow", "s"."Name",
       "e"."Id", "e"."IsAvailable", "e"."Name"
FROM "Borrowings" AS "b"
INNER JOIN "Students" AS "s" ON "b"."StudentId" = "s"."Id"
INNER JOIN "Equipment" AS "e" ON "b"."EquipmentId" = "e"."Id"
WHERE "b"."Status" = 0
ORDER BY "b"."ExpectedReturnDate"
```

Explanation: the query selects active borrowing rows and joins the related student and equipment rows. It is no-tracking because this list is only shown in the UI. The return workflow separately reloads the selected borrowing before changing its state.

## Tracking choice

- Equipment and student lookups, available equipment, and active borrowing lists use `AsNoTracking()` because the returned data is read-only.
- The return repository reloads the target borrowing and its equipment, then explicitly marks the changed status and availability fields for update.
- `CountActiveByStudentAsync` uses SQLite `COUNT` through EF Core's `CountAsync` translation.
