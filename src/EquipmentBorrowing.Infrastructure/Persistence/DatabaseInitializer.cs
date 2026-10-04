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
                new Student(1, "Alice Reyes", true),
                new Student(2, "Bob Santos", true),
                new Student(3, "Carlos Mendoza", false));
        }

        if (!await dbContext.Equipment.AnyAsync(cancellationToken))
        {
            dbContext.Equipment.AddRange(
                new Equipment { Id = 1, Name = "Oscilloscope" },
                new Equipment { Id = 2, Name = "Multimeter" },
                new Equipment { Id = 3, Name = "Soldering Iron" });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
