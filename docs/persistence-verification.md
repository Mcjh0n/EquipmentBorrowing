# Persistence Verification

Verification performed on 2026-10-04 against the SQLite database configured for the application.

- Applied `InitialCreate` with the EF Core migration tool.
- Confirmed `Students`, `Equipment`, `Borrowings`, and `__EFMigrationsHistory` exist. SQLite also created its migration lock and sequence tables.
- Initialized sample students and equipment without clearing existing records.
- Used `BorrowEquipmentService` with the EF repositories to borrow available equipment.
- Reloaded the new borrowing through fresh repository contexts and confirmed it was still active.
- Used `ReturnEquipmentService`, then loaded the record and equipment through fresh contexts; the borrowing was `Returned` and the equipment was available again.
- Ran `dotnet build EquipmentBorrowing.slnx`; the solution built with zero warnings and zero errors.

The Avalonia presentation check can be shown by borrowing an item in the Desktop app, closing and reopening it, then returning the item and reopening the app again. The database/service verification above exercises the same application services and EF repositories without depending on an in-memory repository.
