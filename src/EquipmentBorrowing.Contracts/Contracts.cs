using System.ComponentModel.DataAnnotations;
namespace EquipmentBorrowing.Contracts;

public record StudentDto(int Id, string StudentNumber, string FullName, string? Contact, bool IsEligible);
public record EquipmentDto(int Id, string EquipmentCode, string Name, string? Description, bool IsAvailable);
public record BorrowingDto(int Id, int StudentId, int EquipmentId, DateOnly BorrowedDate, DateOnly ExpectedReturnDate, DateOnly? ReturnedDate, string Status, StudentDto Student, EquipmentDto Equipment);
public record StudentRequest([Required] string StudentNumber, [Required] string FullName, string? Contact);
public record EquipmentRequest([Required] string EquipmentCode, [Required] string Name, string? Description);
public record EligibilityRequest([Required] bool? IsEligible);
public record BorrowRequest([Required] int? StudentId, [Required] int? EquipmentId, [Required] DateOnly? ExpectedReturnDate);
public record CheckoutRequest([Required] int? StudentId, [Required] int[] EquipmentIds, [Required] DateOnly? ExpectedReturnDate);
public record ReturnRequest;
public record CheckoutResponse(IReadOnlyList<BorrowingDto> Borrowings);
public record ApiError(string Code, string Message);
