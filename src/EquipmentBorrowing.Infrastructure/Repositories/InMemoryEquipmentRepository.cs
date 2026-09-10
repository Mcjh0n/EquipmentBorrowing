using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly List<Equipment> _equipment = new()
    {
        new Equipment { Id = 1, Name = "Oscilloscope" },
        new Equipment { Id = 2, Name = "Multimeter" },
        new Equipment { Id = 3, Name = "Soldering Iron" }
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
}