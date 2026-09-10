using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryBorrowingRepository : IBorrowingRepository
{
    private readonly List<Borrowing> _borrowings = new();

    public Task AddAsync(
        Borrowing borrowing,
        CancellationToken cancellationToken = default)
    {
        _borrowings.Add(borrowing);
        return Task.CompletedTask;
    }

    public Task<int> CountActiveByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        int count = _borrowings.Count(
            b => b.Student.Id == studentId &&
                 b.Status == BorrowingStatus.Active);

        return Task.FromResult(count);
    }

    public Task<Borrowing?> GetActiveBorrowingAsync(
        int studentId,
        int equipmentId,
        CancellationToken cancellationToken = default)
    {
        Borrowing? borrowing = _borrowings.FirstOrDefault(
            b => b.Student.Id == studentId &&
                 b.Equipment.Id == equipmentId &&
                 b.Status == BorrowingStatus.Active);

        return Task.FromResult(borrowing);
    }
}