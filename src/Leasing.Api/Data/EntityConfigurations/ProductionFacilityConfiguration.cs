using Leasing.Api.Common;
using Leasing.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leasing.Api.Data.EntityConfigurations;

public class ProductionFacilityConfiguration : IEntityTypeConfiguration<ProductionFacility>
{
    public void Configure(EntityTypeBuilder<ProductionFacility> builder)
    {
        builder.HasKey(x => x.Code);
        
        builder.Property(x => x.Name)
            .HasMaxLength(Constants.ProductionFacilityNameMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Area).IsRequired();
        
        builder.HasCheckConstraint(
            "CK_ProductionFacility_Area",
            $"[Area] >= {Constants.MinProductionFacilityArea}");

    }
}