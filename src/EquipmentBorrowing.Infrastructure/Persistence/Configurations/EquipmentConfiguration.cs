using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Name)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(item => item.IsAvailable)
            .IsRequired();
    }
}
