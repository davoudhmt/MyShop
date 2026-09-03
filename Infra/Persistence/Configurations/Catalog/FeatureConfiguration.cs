using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت Feature برای Entity Framework Core
    /// </summary>
    public class FeatureConfiguration : BaseEntityConfiguration<Feature>
    {
        /// <summary>
        /// متد کانفیگ جدول Feature
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<Feature> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("Features");

            // ===== فیلد Name =====
            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);

            // ایندکس یکتا روی Name
            builder.HasIndex(f => f.Name)
                .IsUnique()
                .HasDatabaseName("UX_Features_Name");

            // ===== فیلد Multiplicity (enum به صورت رشته) =====
            builder.Property(f => f.Multiplicity)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // ===== رابطه چند‌به‌چند با ProductType =====
            builder.HasMany(f => f.ProductTypes)
                .WithMany(pt => pt.Features)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductTypeFeatures",
                    right => right.HasOne<ProductType>()
                        .WithMany()
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Feature>()
                        .WithMany()
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("ProductTypeFeatures");
                        join.HasKey("ProductTypeId", "FeatureId");
                        join.HasIndex("FeatureId")
                            .HasDatabaseName("IX_ProductTypeFeatures_FeatureId");
                    });

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}