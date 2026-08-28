Console.WriteLine("Hello, World!");


// src/EquipmentBorrowing.ConsoleDemo/Program.cs

// using EquipmentBorrowing.Application.Services;
// using EquipmentBorrowing.Infrastructure.Repositories;

// // --- Wire up dependencies manually (no DI container needed) ---
// var studentRepository    = new InMemoryStudentRepository();
// var equipmentRepository  = new InMemoryEquipmentRepository();
// var borrowingRepository  = new InMemoryBorrowingRepository();

// var borrowService = new BorrowEquipmentService(
//     studentRepository,
//     equipmentRepository,
//     borrowingRepository);

// var returnDate = DateTime.Now.AddDays(7);

// Console.WriteLine("===========================================");
// Console.WriteLine("  Campus Equipment Borrowing System Demo  ");
// Console.WriteLine("===========================================\n");

// // ✅ SUCCESS CASE 1: Valid student borrows available equipment
// Console.WriteLine(">>> CASE 1: Alice (authorized) borrows the Oscilloscope");
// var result1 = await borrowService.ExecuteAsync(
//     studentId: 1,
//     equipmentId: 1,
//     expectedReturnDate: returnDate);

// Console.WriteLine(result1.IsSuccess ? $"✅ SUCCESS: {result1.Message}" : $"❌ FAILED:  {result1.Message}");

// Console.WriteLine();

// // ❌ FAILURE CASE 1: Equipment already borrowed (not available)
// Console.WriteLine(">>> CASE 2: Bob tries to borrow the same Oscilloscope (already borrowed)");
// var result2 = await borrowService.ExecuteAsync(
//     studentId: 2,
//     equipmentId: 1,
//     expectedReturnDate: returnDate);

// Console.WriteLine(result2.IsSuccess ? $"✅ SUCCESS: {result2.Message}" : $"❌ FAILED:  {result2.Message}");

// Console.WriteLine();

// // ❌ FAILURE CASE 2: Student not allowed to borrow
// Console.WriteLine(">>> CASE 3: Carlos (not authorized) tries to borrow the Multimeter");
// var result3 = await borrowService.ExecuteAsync(
//     studentId: 3,
//     equipmentId: 2,
//     expectedReturnDate: returnDate);

// Console.WriteLine(result3.IsSuccess ? $"✅ SUCCESS: {result3.Message}" : $"❌ FAILED:  {result3.Message}");

// Console.WriteLine();

// // ❌ FAILURE CASE 3: Equipment does not exist
// Console.WriteLine(">>> CASE 4: Alice tries to borrow Equipment ID 999 (does not exist)");
// var result4 = await borrowService.ExecuteAsync(
//     studentId: 1,
//     equipmentId: 999,
//     expectedReturnDate: returnDate);

// Console.WriteLine(result4.IsSuccess ? $"✅ SUCCESS: {result4.Message}" : $"❌ FAILED:  {result4.Message}");

// Console.WriteLine();

// // ✅ SUCCESS CASE 2: Bob borrows a different available equipment
// Console.WriteLine(">>> CASE 5: Bob (authorized) borrows the Multimeter");
// var result5 = await borrowService.ExecuteAsync(
//     studentId: 2,
//     equipmentId: 2,
//     expectedReturnDate: returnDate);

// Console.WriteLine(result5.IsSuccess ? $"✅ SUCCESS: {result5.Message}" : $"❌ FAILED:  {result5.Message}");

// Console.WriteLine("\n===========================================");
// Console.WriteLine("             Demo Complete                 ");
// Console.WriteLine("===========================================");