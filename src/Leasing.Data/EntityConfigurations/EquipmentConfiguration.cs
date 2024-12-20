﻿using Leasing.Domain.Common;
using Leasing.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leasing.Data.EntityConfigurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(x => x.Code);

        builder.Property(x => x.Name)
            .HasMaxLength(Constants.EquipmentNameMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Area)
            .IsRequired()
            .HasDefaultValue(Constants.MinEquipmentArea);

        builder.HasCheckConstraint(
            "CK_Equipment_Area",
            $"[Area] >= {Constants.MinEquipmentArea}");
    }
}