using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class BorrowEquipmentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    private const int MaxActiveBorrowings = 3;

    public BorrowEquipmentService(
        IStudentRepository studentRepository,
        IEquipmentRepository equipmentRepository,
        IBorrowingRepository borrowingRepository)
    {
        _studentRepository = studentRepository;
        _equipmentRepository = equipmentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task<BorrowingResult> ExecuteAsync(
        int studentId,
        int equipmentId,
        DateTime expectedReturnDate,
        CancellationToken cancellationToken = default)
    {
        // 1. Does the student exist?
        Student? student = await _studentRepository.GetByIdAsync(
            studentId, cancellationToken);

        if (student is null)
            return BorrowingResult.Failure("Student not found.");

        // 2. Is the student allowed to borrow?
        if (!student.IsAllowedToBorrow)
            return BorrowingResult.Failure("Student is not allowed to borrow equipment.");

        // 3. Does the equipment exist?
        Equipment? equipment = await _equipmentRepository.GetByIdAsync(
            equipmentId, cancellationToken);

        if (equipment is null)
            return BorrowingResult.Failure("Equipment not found.");

        // 4. Is the equipment currently available?
        if (!equipment.IsAvailable)
            return BorrowingResult.Failure("Equipment is not available.");

        // 5. Has the student reached the maximum active borrowings?
        int activeCount = await _borrowingRepository.CountActiveByStudentAsync(
            studentId, cancellationToken);

        if (activeCount >= MaxActiveBorrowings)
            return BorrowingResult.Failure(
                $"Student has reached the maximum of {MaxActiveBorrowings} active borrowings.");

        // 6. All rules passed — create the borrowing record
        equipment.MarkAsUnavailable();

        var borrowing = new Borrowing
        {
            Id = Random.Shared.Next(1000, 9999),
            Student = student,
            Equipment = equipment,
            DateBorrowed = DateTime.Now,
            ExpectedReturnDate = expectedReturnDate,
        };

        await _borrowingRepository.AddAsync(borrowing, cancellationToken);

        return BorrowingResult.Success(
            $"Borrowing approved. '{equipment.Name}' borrowed by {student.Name} until {expectedReturnDate:MMMM dd, yyyy}.");
    }
}