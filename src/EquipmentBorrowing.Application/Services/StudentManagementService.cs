using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public sealed class StudentManagementService(IStudentRepository students)
{
    public async Task<Student> RegisterAsync(string number, string name, string? contact, CancellationToken ct = default)
    {
        number = number.Trim(); name = name.Trim(); contact = string.IsNullOrWhiteSpace(contact) ? null : contact.Trim();
        Validate(number, name, contact);
        if (await students.IsStudentNumberInUseAsync(Normalize(number), cancellationToken: ct))
            throw new InvalidOperationException("That student number is already registered.");
        return await students.AddAsync(new Student(0, number, name, contact), ct);
    }

    public async Task UpdateAsync(int id, string number, string name, string? contact, CancellationToken ct = default)
    {
        number = number.Trim(); name = name.Trim(); contact = string.IsNullOrWhiteSpace(contact) ? null : contact.Trim();
        Validate(number, name, contact);
        var student = await students.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Student no longer exists.");
        if (await students.IsStudentNumberInUseAsync(Normalize(number), id, ct))
            throw new InvalidOperationException("That student number is already registered.");
        student.UpdateInformation(number, name, contact);
        await students.UpdateAsync(student, ct);
    }

    public async Task SetEligibilityAsync(int id, bool allowed, CancellationToken ct = default)
    {
        var student = await students.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Student no longer exists.");
        student.SetBorrowingEligibility(allowed);
        await students.UpdateAsync(student, ct);
    }

    private static void Validate(string number, string name, string? contact)
    {
        if (number.Length is 0 or > 40) throw new ArgumentException("Student number is required and must be at most 40 characters.");
        if (name.Length is 0 or > 120) throw new ArgumentException("Full name is required and must be at most 120 characters.");
        if (contact?.Length > 200) throw new ArgumentException("Contact information must be at most 200 characters.");
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
