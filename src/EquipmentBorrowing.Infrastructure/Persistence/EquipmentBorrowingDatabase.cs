namespace EquipmentBorrowing.Infrastructure.Persistence;

public static class EquipmentBorrowingDatabase
{
    public static string GetDatabasePath()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EquipmentBorrowing");

        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "equipment-borrowing.db");
    }

    public static string GetConnectionString() => $"Data Source={GetDatabasePath()}";
}
