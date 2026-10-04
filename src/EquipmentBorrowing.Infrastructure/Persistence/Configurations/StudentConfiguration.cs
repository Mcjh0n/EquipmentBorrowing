using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(student => student.Id);
        builder.Property(student => student.Name)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(student => student.IsAllowedToBorrow)
            .IsRequired();
    }
}
