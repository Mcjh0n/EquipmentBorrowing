using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class EquipmentManagementService(IEquipmentRepository equipment)
{
    public async Task<Equipment> RegisterAsync(string identifier, string name, string? description, CancellationToken ct = default)
    {
        identifier = identifier.Trim(); name = name.Trim(); description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Validate(identifier, name, description);
        if (await equipment.IsIdentifierInUseAsync(Normalize(identifier), cancellationToken: ct))
            throw new EquipmentBorrowing.Application.BusinessConflictException("That equipment identifier is already registered.");
        return await equipment.AddAsync(new Equipment(0, identifier, name, description), ct);
    }

    public async Task UpdateAsync(int id, string identifier, string name, string? description, CancellationToken ct = default)
    {
        identifier = identifier.Trim(); name = name.Trim(); description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Validate(identifier, name, description);
        var item = await equipment.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Equipment no longer exists.");
        if (await equipment.IsIdentifierInUseAsync(Normalize(identifier), id, ct))
            throw new EquipmentBorrowing.Application.BusinessConflictException("That equipment identifier is already registered.");
        item.UpdateInformation(identifier, name, description);
        await equipment.UpdateAsync(item, ct);
    }

    private static void Validate(string identifier, string name, string? description)
    {
        if (identifier.Length is 0 or > 60) throw new ArgumentException("Equipment identifier is required and must be at most 60 characters.");
        if (name.Length is 0 or > 120) throw new ArgumentException("Equipment name is required and must be at most 120 characters.");
        if (description?.Length > 300) throw new ArgumentException("Description must be at most 300 characters.");
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}

