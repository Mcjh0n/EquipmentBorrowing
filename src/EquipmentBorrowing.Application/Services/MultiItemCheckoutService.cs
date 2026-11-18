using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class MultiItemCheckoutService(IBorrowingRepository borrowings)
{
    public async Task<IReadOnlyList<Borrowing>> ExecuteAsync(int studentId, IReadOnlyList<int> equipmentIds, DateOnly dueDate, CancellationToken ct = default)
    {
        if (equipmentIds.Count < 2) throw new InvalidOperationException("Select at least two equipment items.");
        if (equipmentIds.Distinct().Count() != equipmentIds.Count) throw new InvalidOperationException("Duplicate equipment selection is not allowed.");
        return await borrowings.CheckoutManyAsync(studentId, equipmentIds, dueDate, ct);
    }
}
