using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfBorrowingRepository(
    IDbContextFactory<EquipmentBorrowingDbContext> contextFactory) : IBorrowingRepository
{
    public async Task AddAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        await CheckoutManyAsync(borrowing.Student.Id, [borrowing.Equipment.Id], borrowing.ExpectedReturnDate, cancellationToken);
    }

    public async Task UpdateAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var tx = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var changed = await dbContext.Borrowings
            .Where(item => item.Id == borrowing.Id && item.Status == BorrowingStatus.Active)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.Status, BorrowingStatus.Returned)
                .SetProperty(item => item.ReturnedDate, borrowing.ReturnedDate), cancellationToken);
        if (changed != 1)
            throw new EquipmentBorrowing.Application.BusinessConflictException("This borrowing is no longer active. Refresh the list before trying again.");
        var updatedEquipment = await dbContext.Equipment
            .Where(item => item.Id == borrowing.Equipment.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.IsAvailable, true), cancellationToken);
        if (updatedEquipment != 1) throw new EquipmentBorrowing.Application.BusinessConflictException("The equipment record no longer exists; the return was not committed.");
        try { await tx.CommitAsync(cancellationToken); }
        catch (Exception ex)
        {
            throw new CommitOutcomeUnknownException("The return commit could not be confirmed. Do not retry yet; reconnect and refresh the borrowing list first.", ex);
        }
    }

    public async Task<int> CountActiveByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Borrowings
            .CountAsync(
                borrowing => EF.Property<int>(borrowing, "StudentId") == studentId
                    && borrowing.Status == BorrowingStatus.Active,
                cancellationToken);
    }

    public async Task<Borrowing?> GetActiveBorrowingAsync(
        int studentId,
        int equipmentId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Borrowings
            .AsNoTracking()
            .Include(borrowing => borrowing.Student)
            .Include(borrowing => borrowing.Equipment)
            .FirstOrDefaultAsync(
                borrowing => EF.Property<int>(borrowing, "StudentId") == studentId
                    && EF.Property<int>(borrowing, "EquipmentId") == equipmentId
                    && borrowing.Status == BorrowingStatus.Active,
                cancellationToken);
    }

    public async Task<Borrowing?> GetByIdAsync(
        int borrowingId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Borrowings
            .AsNoTracking()
            .Include(borrowing => borrowing.Student)
            .Include(borrowing => borrowing.Equipment)
            .FirstOrDefaultAsync(borrowing => borrowing.Id == borrowingId, cancellationToken);
    }

    public async Task<IEnumerable<Borrowing>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Borrowings
            .AsNoTracking()
            .Include(borrowing => borrowing.Student)
            .Include(borrowing => borrowing.Equipment)
            .Where(borrowing => borrowing.Status == BorrowingStatus.Active)
            .OrderBy(borrowing => borrowing.ExpectedReturnDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Borrowing>> CheckoutManyAsync(
        int studentId, IReadOnlyList<int> equipmentIds, DateOnly expectedReturnDate,
        CancellationToken cancellationToken = default)
    {
        if (equipmentIds.Count == 0 || equipmentIds.Distinct().Count() != equipmentIds.Count)
            throw new EquipmentBorrowing.Application.BusinessConflictException("Select one or more distinct equipment items.");
        if (expectedReturnDate <= DateOnly.FromDateTime(DateTime.Today))
            throw new EquipmentBorrowing.Application.BusinessConflictException("Expected return date must be later than today.");

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var student = await db.Students.SingleOrDefaultAsync(s => s.Id == studentId, cancellationToken)
                ?? throw new KeyNotFoundException("Student no longer exists.");
            if (!student.IsAllowedToBorrow) throw new EquipmentBorrowing.Application.BusinessConflictException("Student is not allowed to borrow.");
            var activeCount = await db.Borrowings.CountAsync(b => EF.Property<int>(b, "StudentId") == studentId && b.Status == BorrowingStatus.Active, cancellationToken);
            if (activeCount + equipmentIds.Count > 3) throw new EquipmentBorrowing.Application.BusinessConflictException("This checkout would exceed the limit of three active items.");

            var items = await db.Equipment.Where(e => equipmentIds.Contains(e.Id)).OrderBy(e => e.Id).ToListAsync(cancellationToken);
            if (items.Count != equipmentIds.Count) throw new KeyNotFoundException("One or more selected equipment records no longer exist.");
            if (items.Any(e => !e.IsAvailable)) throw new EquipmentBorrowing.Application.BusinessConflictException("One or more selected items are no longer available. Refresh and try again.");
            var alreadyActive = await db.Borrowings.AnyAsync(b => b.Status == BorrowingStatus.Active
                && equipmentIds.Contains(EF.Property<int>(b, "EquipmentId")), cancellationToken);
            if (alreadyActive) throw new EquipmentBorrowing.Application.BusinessConflictException("One or more selected items already have an active borrowing. Refresh the equipment list.");

            foreach (var item in items)
            {
                item.MarkAsUnavailable();
                db.Borrowings.Add(new Borrowing
                {
                    Student = student,
                    Equipment = item,
                    DateBorrowed = DateOnly.FromDateTime(DateTime.Today),
                    ExpectedReturnDate = expectedReturnDate
                });
            }

            await db.SaveChangesAsync(cancellationToken);
            try
            {
                await tx.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new CommitOutcomeUnknownException(
                    "The checkout commit could not be confirmed. Do not retry yet; reconnect and refresh the borrowing list first.", ex);
            }
            return await db.Borrowings.AsNoTracking().Include(b => b.Student).Include(b => b.Equipment)
                .Where(b => equipmentIds.Contains(EF.Property<int>(b, "EquipmentId")) && EF.Property<int>(b, "StudentId") == studentId && b.Status == BorrowingStatus.Active)
                .OrderBy(b => b.Id).ToListAsync(cancellationToken);
        }
        catch (CommitOutcomeUnknownException)
        {
            throw;
        }
        catch
        {
            await tx.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<IEnumerable<Borrowing>> GetStudentHistoryAsync(int studentId, CancellationToken ct = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(ct);
        return await db.Borrowings.AsNoTracking().Include(b => b.Student).Include(b => b.Equipment)
            .Where(b => EF.Property<int>(b, "StudentId") == studentId).OrderByDescending(b => b.DateBorrowed).ToListAsync(ct);
    }

    public async Task<IEnumerable<Borrowing>> GetEquipmentHistoryAsync(int equipmentId, CancellationToken ct = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(ct);
        return await db.Borrowings.AsNoTracking().Include(b => b.Student).Include(b => b.Equipment)
            .Where(b => EF.Property<int>(b, "EquipmentId") == equipmentId).OrderByDescending(b => b.DateBorrowed).ToListAsync(ct);
    }

    public async Task<IEnumerable<Borrowing>> SearchAsync(string? search, BorrowingStatus? status, string sortField, bool descending, CancellationToken ct = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(ct);
        var query = db.Borrowings.AsNoTracking().Include(b => b.Student).Include(b => b.Equipment).AsQueryable();
        if (status.HasValue) query = query.Where(b => b.Status == status.Value);
        var term = search?.Trim();
        if (!string.IsNullOrEmpty(term))
        {
            var pattern = $"%{term}%";
            query = query.Where(b => EF.Functions.ILike(b.Student.Name, pattern)
                || EF.Functions.ILike(b.Student.StudentNumber, pattern)
                || EF.Functions.ILike(b.Equipment.Name, pattern));
        }
        var ordered = sortField == "ExpectedReturnDate"
            ? descending ? query.OrderByDescending(b => b.ExpectedReturnDate) : query.OrderBy(b => b.ExpectedReturnDate)
            : descending ? query.OrderByDescending(b => b.DateBorrowed) : query.OrderBy(b => b.DateBorrowed);
        return await ordered.ThenBy(b => b.Id).ToListAsync(ct);
    }
}

