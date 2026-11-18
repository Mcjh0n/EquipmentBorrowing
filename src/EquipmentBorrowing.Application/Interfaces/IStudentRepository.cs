
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Student>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Student> AddAsync(Student student, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
    Task<bool> IsStudentNumberInUseAsync(string normalizedNumber, int? exceptId = null, CancellationToken cancellationToken = default);
}
