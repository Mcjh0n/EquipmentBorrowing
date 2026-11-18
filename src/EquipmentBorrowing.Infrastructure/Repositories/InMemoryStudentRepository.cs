using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new()
    {
        new Student(1, "S-001", "Alice Reyes"),
        new Student(2, "S-002", "Bob Santos"),
        new Student(3, "S-003", "Carlos Mendoza", isAllowedToBorrow: false)
    };

    public Task<Student?> GetByIdAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        Student? student = _students.FirstOrDefault(s => s.Id == studentId);
        return Task.FromResult(student);
    }

    public Task<IEnumerable<Student>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Student>>(_students);
    }

    public Task<Student> AddAsync(Student student, CancellationToken cancellationToken = default) => throw new NotSupportedException("Activity 4 management uses PostgreSQL repositories.");
    public Task UpdateAsync(Student student, CancellationToken cancellationToken = default) => throw new NotSupportedException("Activity 4 management uses PostgreSQL repositories.");
    public Task<bool> IsStudentNumberInUseAsync(string normalizedNumber, int? exceptId = null, CancellationToken cancellationToken = default) => Task.FromResult(_students.Any(s => s.StudentNumberNormalized == normalizedNumber && s.Id != exceptId));
}
