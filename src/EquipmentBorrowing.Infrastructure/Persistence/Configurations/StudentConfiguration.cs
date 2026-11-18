using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students", table =>
        {
            table.HasCheckConstraint("CK_Students_StudentNumber_Normalized", "\"StudentNumberNormalized\" = upper(btrim(\"StudentNumber\"))");
            table.HasCheckConstraint("CK_Students_Name_NotBlank", "length(btrim(\"Name\")) > 0");
            table.HasCheckConstraint("CK_Students_Contact_NotBlank", "\"ContactInformation\" IS NULL OR length(btrim(\"ContactInformation\")) > 0");
        });
        builder.HasKey(student => student.Id);
        builder.Property(student => student.StudentNumber).HasMaxLength(40).IsRequired();
        builder.Property(student => student.StudentNumberNormalized).HasMaxLength(40).IsRequired();
        builder.HasIndex(student => student.StudentNumberNormalized).IsUnique();
        builder.Property(student => student.Name)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(student => student.ContactInformation).HasMaxLength(200);
        builder.Property(student => student.IsAllowedToBorrow)
            .IsRequired();
    }
}
