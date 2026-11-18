using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class BorrowingLookupService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    public BorrowingLookupService(
        IStudentRepository studentRepository,
        IEquipmentRepository equipmentRepository,
        IBorrowingRepository borrowingRepository)
    {
        _studentRepository = studentRepository;
        _equipmentRepository = equipmentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public Task<IEnumerable<Student>> GetStudentsAsync(
        CancellationToken cancellationToken = default)
    {
        return _studentRepository.GetAllAsync(cancellationToken);
    }

    public Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync(
        CancellationToken cancellationToken = default)
    {
        return _equipmentRepository.GetAvailableAsync(cancellationToken);
    }

    public Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync(
        CancellationToken cancellationToken = default)
    {
        return _borrowingRepository.GetActiveAsync(cancellationToken);
    }

    public Task<IEnumerable<Equipment>> GetEquipmentAsync(CancellationToken ct = default) => _equipmentRepository.GetAllAsync(ct);
    public async Task<IEnumerable<Borrowing>> GetStudentHistoryAsync(int id, CancellationToken ct = default)
    {
        if (await _studentRepository.GetByIdAsync(id, ct) is null) throw new KeyNotFoundException("The selected student no longer exists. Refresh the list.");
        return await _borrowingRepository.GetStudentHistoryAsync(id, ct);
    }
    public async Task<IEnumerable<Borrowing>> GetEquipmentHistoryAsync(int id, CancellationToken ct = default)
    {
        if (await _equipmentRepository.GetByIdAsync(id, ct) is null) throw new KeyNotFoundException("The selected equipment no longer exists. Refresh the list.");
        return await _borrowingRepository.GetEquipmentHistoryAsync(id, ct);
    }
    public Task<IEnumerable<Borrowing>> SearchBorrowingsAsync(string? text, BorrowingStatus? status, string sort, bool descending, CancellationToken ct = default) =>
        _borrowingRepository.SearchAsync(text, status, sort, descending, ct);
}
