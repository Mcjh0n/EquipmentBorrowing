using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public sealed class EfStudentRepository(IDbContextFactory<EquipmentBorrowingDbContext> factory) : IStudentRepository
{
    public async Task<Student?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }
    public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Students.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    }
    public async Task<Student> AddAsync(Student student, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        db.Students.Add(student); await db.SaveChangesAsync(ct); return student;
    }
    public async Task UpdateAsync(Student student, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        db.Students.Attach(student);
        db.Entry(student).Property(x => x.StudentNumber).IsModified = true;
        db.Entry(student).Property(x => x.StudentNumberNormalized).IsModified = true;
        db.Entry(student).Property(x => x.Name).IsModified = true;
        db.Entry(student).Property(x => x.ContactInformation).IsModified = true;
        db.Entry(student).Property(x => x.IsAllowedToBorrow).IsModified = true;
        await db.SaveChangesAsync(ct);
    }
    public async Task<bool> IsStudentNumberInUseAsync(string number, int? exceptId = null, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.Students.AsNoTracking().Where(x => x.StudentNumberNormalized == number);
        if (exceptId.HasValue) query = query.Where(x => x.Id != exceptId.Value);
        return await query.AnyAsync(ct);
    }
}
