using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new()
    {
        new Student(1, "Alice Reyes", IsAllowedToBorrow: true),
        new Student(2, "Bob Santos", IsAllowedToBorrow: true),
        new Student(3, "Carlos Mendoza", IsAllowedToBorrow: false)
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
}
