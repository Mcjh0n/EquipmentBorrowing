using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public sealed class EquipmentBorrowingDbContextFactory
    : IDesignTimeDbContextFactory<EquipmentBorrowingDbContext>
{
    public EquipmentBorrowingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable("EquipmentBorrowing__ConnectionString")
                ?? "Host=localhost;Database=equipment_borrowing;Username=postgres;Password=postgres")
            .Options;

        return new EquipmentBorrowingDbContext(options);
    }
}
