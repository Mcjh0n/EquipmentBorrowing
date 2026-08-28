namespace EquipmentBorrowing.Domain;

public record Student(
    int Id,
    string Name,
    bool IsAllowedToBorrow
);