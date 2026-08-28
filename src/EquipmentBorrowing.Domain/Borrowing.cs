namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public int Id { get; init; }
    public Student Student { get; init; } = null!;
    public Equipment Equipment { get; init; } = null!;
    public DateTime DateBorrowed { get; init; }
    public DateTime ExpectedReturnDate { get; init; }
    public BorrowingStatus Status { get; private set; } = BorrowingStatus.Active;

    public void MarkAsReturned()
    {
        Status = BorrowingStatus.Returned;
    }
}