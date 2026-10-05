using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت FeatureValue برای Entity Framework Core
    /// </summary>
    public class FeatureValueConfiguration : BaseEntityConfiguration<FeatureValue>
    {
        public override void Configure(EntityTypeBuilder<FeatureValue> builder)
        {
            builder.ToTable("FeatureValues", "Catalog");

            // ۱. نگاشت صریح فیلدها
            builder.Property(fv => fv.Value).IsRequired().HasMaxLength(100).IsUnicode(true);
            builder.Property(fv => fv.FeatureId).IsRequired();

            // ۲. نگاشت صریح و دوطرفه‌ی رابطه (این خط کلید حل مشکل است)
            // ما دقیقاً به پراپرتی‌های تعریف‌شده در کلاس FeatureValue و Feature اشاره می‌کنیم
            builder.HasOne(fv => fv.Feature)          // پراپرتی ناوبری در FeatureValue
                   .WithMany(f => f.Values)           // پراپرتی ناوبری در Feature
                   .HasForeignKey(fv => fv.FeatureId) // کلید خارجی در FeatureValue
                   .OnDelete(DeleteBehavior.Restrict); // جلوگیری از چرخه حذف

            // ۳. ایندکس‌ها
            builder.HasIndex(fv => fv.FeatureId).HasDatabaseName("IX_FeatureValues_FeatureId");
            builder.HasIndex(fv => new { fv.FeatureId, fv.Value })
                   .IsUnique()
                   .HasDatabaseName("UX_FeatureValues_FeatureId_Value");

            // ۴. رابطه چند‌به‌چند با Variant
            builder.HasMany(fv => fv.Variants)
                .WithMany(v => v.FeatureValues)
                .UsingEntity<Dictionary<string, object>>(
                    "FeatureValueVariants",
                    right => right.HasOne<Variant>().WithMany().HasForeignKey("VariantId").OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<FeatureValue>().WithMany().HasForeignKey("FeatureValueId").OnDelete(DeleteBehavior.Restrict),
                    join => {
                        join.ToTable("FeatureValueVariants", "Catalog");
                        join.HasKey("FeatureValueId", "VariantId");
                        join.HasIndex("VariantId").HasDatabaseName("IX_FeatureValueVariants_VariantId");
                    });

            ConfigureBaseEntity(builder);
        }
    }
}