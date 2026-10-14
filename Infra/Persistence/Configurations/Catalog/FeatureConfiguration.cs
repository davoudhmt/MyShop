using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس پیکربندی موجودیت ویژگی (Feature) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس، شامل نام جدول، محدودیت‌های فیلدها، ایندکس‌های یکتا و روابط چندبه‌چند با نوع محصول را تعریف می‌کند.
    /// </summary>
    public class FeatureConfiguration : BaseEntityConfiguration<Feature>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Feature
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Feature> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Features", "Catalog");

            // ۲. پیکربندی فیلدهای اصلی و ایندکس‌های یکتا
            builder.Property(f => f.Name).IsRequired().HasMaxLength(100).IsUnicode(true);
            builder.HasIndex(f => f.Name).IsUnique().HasDatabaseName("UX_Features_Name");

            builder.Property(f => f.Multiplicity).IsRequired().HasConversion<string>().HasMaxLength(20);

            // ۳. پیکربندی رابطه چند‌به‌چند با ProductType
            builder.HasMany(f => f.ProductTypes)
                .WithMany(pt => pt.Features)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductTypeFeatures",
                    right => right.HasOne<ProductType>().WithMany().HasForeignKey("ProductTypeId").OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Feature>().WithMany().HasForeignKey("FeatureId").OnDelete(DeleteBehavior.Restrict), // ✅ جلوگیری از حذف ناخواسته ویژگی هنگام حذف نوع محصول
                    join => {
                        join.ToTable("ProductTypeFeatures", "Catalog");
                        join.HasKey("ProductTypeId", "FeatureId");
                        join.HasIndex("FeatureId").HasDatabaseName("IX_ProductTypeFeatures_FeatureId");
                    });

            // ۴. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، و حذف منطقی)
            ConfigureBaseEntity(builder);
        }
    }
}