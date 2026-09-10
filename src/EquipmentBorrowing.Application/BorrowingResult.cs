namespace EquipmentBorrowing.Application;

public class BorrowingResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = string.Empty;

    public static BorrowingResult Success(string message) =>
        new BorrowingResult { IsSuccess = true, Message = message };

    public static BorrowingResult Failure(string message) =>
        new BorrowingResult { IsSuccess = false, Message = message };
}