namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public int Id { get; set; }
    public Student Student { get; init; } = null!;
    public Equipment Equipment { get; init; } = null!;
    public DateOnly DateBorrowed { get; init; }
    public DateOnly ExpectedReturnDate { get; init; }
    public DateOnly? ReturnedDate { get; private set; }
    public BorrowingStatus Status { get; private set; } = BorrowingStatus.Active;

    public void MarkAsReturned(DateOnly returnedDate)
    {
        if (Status != BorrowingStatus.Active)
            throw new InvalidOperationException("Only active borrowings can be returned.");
        if (returnedDate < DateBorrowed)
            throw new ArgumentOutOfRangeException(nameof(returnedDate), "Return date cannot precede checkout.");
        Status = BorrowingStatus.Returned;
        ReturnedDate = returnedDate;
    }
}
