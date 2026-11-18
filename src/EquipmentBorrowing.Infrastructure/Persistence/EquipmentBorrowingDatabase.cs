namespace EquipmentBorrowing.Infrastructure.Persistence;

public static class EquipmentBorrowingDatabase
{
    public static string GetConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("EquipmentBorrowing__ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Set EquipmentBorrowing__ConnectionString to your local PostgreSQL connection string. See README.md.");
        return connectionString;
    }
}
