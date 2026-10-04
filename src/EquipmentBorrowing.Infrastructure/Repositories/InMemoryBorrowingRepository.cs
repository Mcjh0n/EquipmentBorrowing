using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryBorrowingRepository : IBorrowingRepository
{
    private readonly List<Borrowing> _borrowings = new();
    private int _nextId = 1;

    public Task AddAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        if (borrowing.Id == 0)
            borrowing.Id = _nextId++;

        _borrowings.Add(borrowing);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<int> CountActiveByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        int count = _borrowings.Count(
            borrowing => borrowing.Student.Id == studentId &&
                         borrowing.Status == BorrowingStatus.Active);

        return Task.FromResult(count);
    }

    public Task<Borrowing?> GetActiveBorrowingAsync(
        int studentId,
        int equipmentId,
        CancellationToken cancellationToken = default)
    {
        Borrowing? borrowing = _borrowings.FirstOrDefault(
            borrowing => borrowing.Student.Id == studentId &&
                         borrowing.Equipment.Id == equipmentId &&
                         borrowing.Status == BorrowingStatus.Active);

        return Task.FromResult(borrowing);
    }

    public Task<Borrowing?> GetByIdAsync(
        int borrowingId,
        CancellationToken cancellationToken = default)
    {
        Borrowing? borrowing = _borrowings.FirstOrDefault(
            borrowing => borrowing.Id == borrowingId);

        return Task.FromResult(borrowing);
    }

    public Task<IEnumerable<Borrowing>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Borrowing> activeBorrowings = _borrowings.Where(
            borrowing => borrowing.Status == BorrowingStatus.Active);

        return Task.FromResult(activeBorrowings);
    }
}
