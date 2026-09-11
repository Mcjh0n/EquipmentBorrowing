using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IBorrowingRepository
{
    Task AddAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<Borrowing?> GetActiveBorrowingAsync(
        int studentId,
        int equipmentId,
        CancellationToken cancellationToken = default);

    Task<Borrowing?> GetByIdAsync(
        int borrowingId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Borrowing>> GetActiveAsync(
        CancellationToken cancellationToken = default);
}
