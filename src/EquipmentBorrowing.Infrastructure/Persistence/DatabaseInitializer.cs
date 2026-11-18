using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        EquipmentBorrowingDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (!await dbContext.Students.AnyAsync(cancellationToken))
        {
            dbContext.Students.AddRange(
                new Student(0, "S-001", "Alice Reyes", "alice@example.edu"),
                new Student(0, "S-002", "Bob Santos", "bob@example.edu"),
                new Student(0, "S-003", "Carlos Mendoza", null, false));
        }

        if (!await dbContext.Equipment.AnyAsync(cancellationToken))
        {
            dbContext.Equipment.AddRange(
                new Equipment(0, "LAB-OSC-001", "Oscilloscope", "Digital laboratory oscilloscope"),
                new Equipment(0, "LAB-MUL-001", "Multimeter", "Digital multimeter"),
                new Equipment(0, "LAB-SOL-001", "Soldering Iron", "Temperature-controlled soldering iron"));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
