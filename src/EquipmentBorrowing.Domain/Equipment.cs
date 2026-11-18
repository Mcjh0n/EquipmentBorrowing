namespace EquipmentBorrowing.Domain;

public class Equipment
{
    private Equipment() { }

    public Equipment(int id, string equipmentIdentifier, string name, string? description = null)
    {
        Id = id;
        EquipmentIdentifier = equipmentIdentifier;
        EquipmentIdentifierNormalized = equipmentIdentifier.Trim().ToUpperInvariant();
        Name = name;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public int Id { get; private set; }
    public string EquipmentIdentifier { get; private set; } = string.Empty;
    public string EquipmentIdentifierNormalized { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsAvailable { get; private set; } = true;

    public void UpdateInformation(string identifier, string name, string? description)
    {
        EquipmentIdentifier = identifier;
        EquipmentIdentifierNormalized = identifier.Trim().ToUpperInvariant();
        Name = name;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void SetAvailable(bool available) => IsAvailable = available;
    public override string ToString() => $"{EquipmentIdentifier} - {Name}";

    public void MarkAsUnavailable() => IsAvailable = false;
    public void MarkAsAvailable() => IsAvailable = true;
}
