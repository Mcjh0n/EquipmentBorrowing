using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public sealed class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment", table =>
        {
            table.HasCheckConstraint("CK_Equipment_Identifier_Normalized", "\"EquipmentIdentifierNormalized\" = upper(btrim(\"EquipmentIdentifier\"))");
            table.HasCheckConstraint("CK_Equipment_Name_NotBlank", "length(btrim(\"Name\")) > 0");
            table.HasCheckConstraint("CK_Equipment_Description_NotBlank", "\"Description\" IS NULL OR length(btrim(\"Description\")) > 0");
        });
        builder.HasKey(item => item.Id);
        builder.Property(item => item.EquipmentIdentifier).HasMaxLength(60).IsRequired();
        builder.Property(item => item.EquipmentIdentifierNormalized).HasMaxLength(60).IsRequired();
        builder.HasIndex(item => item.EquipmentIdentifierNormalized).IsUnique();
        builder.Property(item => item.Name)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(item => item.Description).HasMaxLength(300);
        builder.Property(item => item.IsAvailable)
            .IsRequired();
    }
}
