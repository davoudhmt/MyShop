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
        /// <summary>
        /// متد کانفیگ جدول FeatureValue
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<FeatureValue> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("FeatureValues");

            // ===== فیلد Value =====
            builder.Property(fv => fv.Value)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);

            // ===== فیلد FeatureId =====
            builder.Property(fv => fv.FeatureId)
                .IsRequired();

            // ایندکس روی FeatureId برای کوئری‌های سریع
            builder.HasIndex(fv => fv.FeatureId)
                .HasDatabaseName("IX_FeatureValues_FeatureId");

            // ایندکس یکتا ترکیبی (هر مقدار باید در یک ویژگی یکتا باشه)
            // یعنی نمی‌توانیم دو بار "قرمز" را در ویژگی "رنگ" داشته باشیم
            builder.HasIndex(fv => new { fv.FeatureId, fv.Value })
                .IsUnique()
                .HasDatabaseName("UX_FeatureValues_FeatureId_Value");

            // ===== رابطه یک‌به‌چند با Feature =====
            builder.HasOne<Feature>()
                .WithMany(f => f.Values)
                .HasForeignKey(fv => fv.FeatureId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);  // اگه Feature حذف بشه، مقادیرش هم حذف می‌شن

            // ===== رابطه چند‌به‌چند با Variant =====
            builder.HasMany(fv => fv.Variants)
                .WithMany(v => v.FeatureValues)
                .UsingEntity<Dictionary<string, object>>(
                    "FeatureValueVariants",
                    right => right.HasOne<Variant>()
                        .WithMany()
                        .HasForeignKey("VariantId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<FeatureValue>()
                        .WithMany()
                        .HasForeignKey("FeatureValueId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("FeatureValueVariants");
                        join.HasKey("FeatureValueId", "VariantId");
                        join.HasIndex("VariantId")
                            .HasDatabaseName("IX_FeatureValueVariants_VariantId");
                    });

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}