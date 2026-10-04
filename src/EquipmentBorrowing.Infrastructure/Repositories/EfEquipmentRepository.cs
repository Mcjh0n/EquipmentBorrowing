using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfEquipmentRepository(
    IDbContextFactory<EquipmentBorrowingDbContext> contextFactory) : IEquipmentRepository
{
    public async Task<Equipment?> GetByIdAsync(
        int equipmentId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Equipment
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == equipmentId, cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetAvailableAsync(
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Equipment
            .AsNoTracking()
            .Where(item => item.IsAvailable)
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);
    }
}
