using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.ToTable("Borrowings", table =>
        {
            table.HasCheckConstraint("CK_Borrowings_Due_After_Checkout", "\"ExpectedReturnDate\" > \"DateBorrowed\"");
            table.HasCheckConstraint("CK_Borrowings_Return_After_Checkout", "\"ReturnedDate\" IS NULL OR \"ReturnedDate\" >= \"DateBorrowed\"");
            table.HasCheckConstraint("CK_Borrowings_Return_Status", "(\"Status\" = 0 AND \"ReturnedDate\" IS NULL) OR (\"Status\" = 1 AND \"ReturnedDate\" IS NOT NULL)");
        });
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
        builder.HasIndex("EquipmentId").HasFilter("\"Status\" = 0").IsUnique();
        builder.HasIndex(borrowing => borrowing.Status);
    }
}
