using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.ToTable("Borrowings");
        builder.HasKey(borrowing => borrowing.Id);
        builder.Property(borrowing => borrowing.Id).ValueGeneratedOnAdd();
        builder.Property(borrowing => borrowing.DateBorrowed).IsRequired();
        builder.Property(borrowing => borrowing.ExpectedReturnDate).IsRequired();
        builder.Property(borrowing => borrowing.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(borrowing => borrowing.Student)
            .WithMany()
            .HasForeignKey("StudentId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(borrowing => borrowing.Equipment)
            .WithMany()
            .HasForeignKey("EquipmentId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex("StudentId");
        builder.HasIndex("EquipmentId");
        builder.HasIndex(borrowing => borrowing.Status);
    }
}
