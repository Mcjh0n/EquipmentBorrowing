using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfStudentRepository(
    IDbContextFactory<EquipmentBorrowingDbContext> contextFactory) : IStudentRepository
{
    public async Task<Student?> GetByIdAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == studentId, cancellationToken);
    }

    public async Task<IEnumerable<Student>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Students
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .ToListAsync(cancellationToken);
    }
}
