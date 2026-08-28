namespace EquipmentBorrowing.Domain;

public class Equipment
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsAvailable { get; private set; } = true;

    public void MarkAsUnavailable() => IsAvailable = false;
    public void MarkAsAvailable() => IsAvailable = true;
}