namespace EquipmentBorrowing.Domain;

public class Student
{
    private Student() { }

    public Student(int id, string studentNumber, string name, string? contactInformation = null, bool isAllowedToBorrow = true)
    {
        Id = id;
        StudentNumber = studentNumber;
        StudentNumberNormalized = studentNumber.Trim().ToUpperInvariant();
        Name = name;
        ContactInformation = contactInformation;
        IsAllowedToBorrow = isAllowedToBorrow;
    }

    public int Id { get; private set; }
    public string StudentNumber { get; private set; } = string.Empty;
    public string StudentNumberNormalized { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? ContactInformation { get; private set; }
    public bool IsAllowedToBorrow { get; private set; }

    public void UpdateInformation(string studentNumber, string name, string? contactInformation)
    {
        StudentNumber = studentNumber;
        StudentNumberNormalized = studentNumber.Trim().ToUpperInvariant();
        Name = name;
        ContactInformation = string.IsNullOrWhiteSpace(contactInformation) ? null : contactInformation.Trim();
    }

    public void SetBorrowingEligibility(bool isAllowed) => IsAllowedToBorrow = isAllowed;
    public override string ToString() => $"{StudentNumber} - {Name}";
}
