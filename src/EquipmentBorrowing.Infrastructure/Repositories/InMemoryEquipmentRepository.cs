using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly List<Equipment> _equipment = new()
    {
        new Equipment(1, "LAB-OSC-001", "Oscilloscope"),
        new Equipment(2, "LAB-MUL-001", "Multimeter"),
        new Equipment(3, "LAB-SOL-001", "Soldering Iron")
    };

    public Task<Equipment?> GetByIdAsync(
        int equipmentId,
        CancellationToken cancellationToken = default)
    {
        Equipment? equipment = _equipment.FirstOrDefault(e => e.Id == equipmentId);
        return Task.FromResult(equipment);
    }

    public Task<IEnumerable<Equipment>> GetAvailableAsync(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Equipment> available = _equipment.Where(e => e.IsAvailable);
        return Task.FromResult(available);
    }

    public Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Equipment>>(_equipment);
    public Task<Equipment> AddAsync(Equipment equipment, CancellationToken cancellationToken = default) => throw new NotSupportedException("Activity 4 management uses PostgreSQL repositories.");
    public Task UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default) => throw new NotSupportedException("Activity 4 management uses PostgreSQL repositories.");
    public Task<bool> IsIdentifierInUseAsync(string normalizedIdentifier, int? exceptId = null, CancellationToken cancellationToken = default) => Task.FromResult(_equipment.Any(e => e.EquipmentIdentifierNormalized == normalizedIdentifier && e.Id != exceptId));
}
