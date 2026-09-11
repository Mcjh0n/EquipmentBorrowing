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
}