using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure.Repositories;

var studentRepository = new InMemoryStudentRepository();
var equipmentRepository = new InMemoryEquipmentRepository();
var borrowingRepository = new InMemoryBorrowingRepository();

var borrowService = new BorrowEquipmentService(
    studentRepository,
    equipmentRepository,
    borrowingRepository);

var expectedReturnDate = DateTime.Today.AddDays(7);

Console.WriteLine("Campus Equipment Borrowing System Demo");
Console.WriteLine(new string('=', 40));
Console.WriteLine($"Expected return date: {expectedReturnDate:MMMM dd, yyyy}");
Console.WriteLine();

var firstBorrowing = await borrowService.ExecuteAsync(
    studentId: 1,
    equipmentId: 1,
    expectedReturnDate: expectedReturnDate);
PrintResult("1. Alice borrows the available Oscilloscope", firstBorrowing.IsSuccess, firstBorrowing.Message);

var unavailableEquipment = await borrowService.ExecuteAsync(
    studentId: 2,
    equipmentId: 1,
    expectedReturnDate: expectedReturnDate);
PrintResult("2. Bob requests the already borrowed Oscilloscope", unavailableEquipment.IsSuccess, unavailableEquipment.Message);

var ineligibleStudent = await borrowService.ExecuteAsync(
    studentId: 3,
    equipmentId: 2,
    expectedReturnDate: expectedReturnDate);
PrintResult("3. Carlos requests the Multimeter", ineligibleStudent.IsSuccess, ineligibleStudent.Message);

var missingEquipment = await borrowService.ExecuteAsync(
    studentId: 1,
    equipmentId: 999,
    expectedReturnDate: expectedReturnDate);
PrintResult("4. Alice requests equipment that does not exist", missingEquipment.IsSuccess, missingEquipment.Message);

var secondBorrowing = await borrowService.ExecuteAsync(
    studentId: 2,
    equipmentId: 2,
    expectedReturnDate: expectedReturnDate);
PrintResult("5. Bob borrows the available Multimeter", secondBorrowing.IsSuccess, secondBorrowing.Message);

static void PrintResult(string scenario, bool isSuccess, string message)
{
    var outcome = isSuccess ? "SUCCESS" : "FAILED";
    Console.WriteLine($"{scenario}\n   {outcome}: {message}\n");
}
