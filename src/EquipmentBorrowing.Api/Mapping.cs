using EquipmentBorrowing.Contracts;
using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Api;
public static class Mapping {
    public static StudentDto Dto(this Student s) => new(s.Id,s.StudentNumber,s.Name,s.ContactInformation,s.IsAllowedToBorrow);
    public static EquipmentDto Dto(this Equipment e) => new(e.Id,e.EquipmentIdentifier,e.Name,e.Description,e.IsAvailable);
    public static BorrowingDto Dto(this Borrowing b) => new(b.Id,b.Student.Id,b.Equipment.Id,b.DateBorrowed,b.ExpectedReturnDate,b.ReturnedDate,b.Status.ToString(),b.Student.Dto(),b.Equipment.Dto());
}
