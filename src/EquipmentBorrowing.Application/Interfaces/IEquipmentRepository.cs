using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(
        int equipmentId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Equipment>> GetAvailableAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Equipment> AddAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task<bool> IsIdentifierInUseAsync(string normalizedIdentifier, int? exceptId = null, CancellationToken cancellationToken = default);
}
