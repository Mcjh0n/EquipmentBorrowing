using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Application.Services;

public sealed class ApiBorrowingService(IBorrowingRepository repository) {
    public Task<IReadOnlyList<Borrowing>> CheckoutAsync(int studentId, int[] equipmentIds, DateOnly due, bool multiple, CancellationToken ct) {
        if(equipmentIds.Length < (multiple ? 2 : 1) || equipmentIds.Distinct().Count()!=equipmentIds.Length)
            throw new ArgumentException("Select the required number of distinct equipment items.");
        if(due<=DateOnly.FromDateTime(DateTime.Today)) throw new ArgumentException("Expected return date must be after the server's local date.");
        return repository.CheckoutManyAsync(studentId,equipmentIds,due,ct);
    }
    public async Task<Borrowing> ReturnAsync(int id,CancellationToken ct) {
        var row=await repository.GetByIdAsync(id,ct) ?? throw new KeyNotFoundException("Borrowing not found.");
        if(row.Status==BorrowingStatus.Returned) throw new BusinessConflictException("This equipment has already been returned.");
        row.MarkAsReturned(DateOnly.FromDateTime(DateTime.Today));
        await repository.UpdateAsync(row,ct);
        return await repository.GetByIdAsync(id,ct) ?? throw new KeyNotFoundException("Borrowing not found after return.");
    }
}
