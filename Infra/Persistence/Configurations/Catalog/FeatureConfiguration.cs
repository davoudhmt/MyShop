using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    public class FeatureConfiguration : BaseEntityConfiguration<Feature>
    {
        public override void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTable("Features", "Catalog");

            builder.Property(f => f.Name).IsRequired().HasMaxLength(100).IsUnicode(true);
            builder.HasIndex(f => f.Name).IsUnique().HasDatabaseName("UX_Features_Name");

            builder.Property(f => f.Multiplicity).IsRequired().HasConversion<string>().HasMaxLength(20);

            // ===== رابطه چند‌به‌چند با ProductType =====
            builder.HasMany(f => f.ProductTypes)
                .WithMany(pt => pt.Features)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductTypeFeatures",
                    right => right.HasOne<ProductType>().WithMany().HasForeignKey("ProductTypeId").OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Feature>().WithMany().HasForeignKey("FeatureId").OnDelete(DeleteBehavior.Restrict), // ✅ تغییر به Restrict
                    join => {
                        join.ToTable("ProductTypeFeatures", "Catalog");
                        join.HasKey("ProductTypeId", "FeatureId");
                        join.HasIndex("FeatureId").HasDatabaseName("IX_ProductTypeFeatures_FeatureId");
                    });

            ConfigureBaseEntity(builder);
        }
    }
}