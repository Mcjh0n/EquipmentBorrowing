using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfEquipmentRepository(IDbContextFactory<EquipmentBorrowingDbContext> factory) : IEquipmentRepository
{
    public async Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Equipment.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }
    public async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Equipment.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    }
    public async Task<IEnumerable<Equipment>> GetAvailableAsync(CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Equipment.AsNoTracking().Where(x => x.IsAvailable).OrderBy(x => x.Name).ToListAsync(ct);
    }
    public async Task<Equipment> AddAsync(Equipment item, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        db.Equipment.Add(item); await db.SaveChangesAsync(ct); return item;
    }
    public async Task UpdateAsync(Equipment item, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        db.Equipment.Attach(item);
        db.Entry(item).Property(x => x.EquipmentIdentifier).IsModified = true;
        db.Entry(item).Property(x => x.EquipmentIdentifierNormalized).IsModified = true;
        db.Entry(item).Property(x => x.Name).IsModified = true;
        db.Entry(item).Property(x => x.Description).IsModified = true;
        await db.SaveChangesAsync(ct);
    }
    public async Task<bool> IsIdentifierInUseAsync(string identifier, int? exceptId = null, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.Equipment.AsNoTracking().Where(x => x.EquipmentIdentifierNormalized == identifier);
        if (exceptId.HasValue) query = query.Where(x => x.Id != exceptId.Value);
        return await query.AnyAsync(ct);
    }
}
