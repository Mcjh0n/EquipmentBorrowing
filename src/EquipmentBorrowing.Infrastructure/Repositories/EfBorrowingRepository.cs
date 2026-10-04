using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfBorrowingRepository(
    IDbContextFactory<EquipmentBorrowingDbContext> contextFactory) : IBorrowingRepository
{
    public async Task AddAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

        dbContext.Attach(borrowing.Student);
        dbContext.Attach(borrowing.Equipment);
        dbContext.Entry(borrowing.Equipment)
            .Property(item => item.IsAvailable)
            .IsModified = true;

        dbContext.Borrowings.Add(borrowing);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

        dbContext.Borrowings.Attach(borrowing);
        dbContext.Entry(borrowing)
            .Property(item => item.Status)
            .IsModified = true;
        dbContext.Entry(borrowing.Equipment)
            .Property(item => item.IsAvailable)
            .IsModified = true;

        await dbContext.SaveChangesAsync(cancellationToken);
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
}
