
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Student>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
