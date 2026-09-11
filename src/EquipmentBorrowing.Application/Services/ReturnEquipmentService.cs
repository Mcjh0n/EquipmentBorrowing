using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class ReturnEquipmentService
{
    private readonly IBorrowingRepository _borrowingRepository;

    public ReturnEquipmentService(IBorrowingRepository borrowingRepository)
    {
        _borrowingRepository = borrowingRepository;
    }

    public async Task<BorrowingResult> ExecuteAsync(
        int borrowingId,
        CancellationToken cancellationToken = default)
    {
        Borrowing? borrowing = await _borrowingRepository.GetByIdAsync(
            borrowingId,
            cancellationToken);

        if (borrowing is null)
        {
            return BorrowingResult.Failure("Borrowing record not found.");
        }

        if (borrowing.Status == BorrowingStatus.Returned)
        {
            return BorrowingResult.Failure("This equipment has already been returned.");
        }

        borrowing.MarkAsReturned();
        borrowing.Equipment.MarkAsAvailable();

        return BorrowingResult.Success(
            $"'{borrowing.Equipment.Name}' was returned successfully.");
    }
}
