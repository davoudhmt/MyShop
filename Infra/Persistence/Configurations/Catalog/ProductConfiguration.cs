using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس پیکربندی موجودیت محصول (Product) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس، شامل نام جدول، محدودیت‌های فیلدها، ایندکس‌ها، 
    /// روابط ارجاعی، Value Objectها و روابط چندبه‌چند را تعریف می‌کند.
    /// </summary>
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Product
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Products", "Catalog");

            // ۲. پیکربندی فیلدهای پایه و ایندکس‌ها
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200).IsUnicode(true);
            builder.HasIndex(p => p.Name).HasDatabaseName("IX_Products_Name");

            builder.Property(p => p.ShortDescription).IsRequired().HasMaxLength(500).IsUnicode(true);
            builder.Property(p => p.Description).IsRequired(false).HasMaxLength(2000).IsUnicode(true);

            // ۳. پیکربندی روابط ارجاعی (جلوگیری از چرخه‌ی حذف با Restrict یا SetNull)
            builder.Property(p => p.CategoryId).IsRequired();
            builder.HasOne<Category>().WithMany().HasForeignKey(p => p.CategoryId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(p => p.CategoryId).HasDatabaseName("IX_Products_CategoryId");

            builder.Property(p => p.BrandId).IsRequired(false);
            builder.HasOne<Brand>().WithMany().HasForeignKey(p => p.BrandId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            builder.HasIndex(p => p.BrandId).HasDatabaseName("IX_Products_BrandId");

            builder.Property(p => p.ProductTypeId).IsRequired();
            builder.HasOne<ProductType>().WithMany().HasForeignKey(p => p.ProductTypeId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(p => p.ProductTypeId).HasDatabaseName("IX_Products_ProductTypeId");

            // ۴. پیکربندی Value Objectها (Owned Types)
            builder.OwnsOne(p => p.ProductCode, pc => {
                pc.Property(c => c.Value).HasColumnName("ProductCode").IsRequired().HasMaxLength(6).IsUnicode(false);
                pc.HasIndex(c => c.Value).IsUnique().HasDatabaseName("UX_Products_ProductCode");
            });

            builder.OwnsOne(p => p.Slug, s => {
                s.Property(v => v.Value).HasColumnName("Slug").IsRequired().HasMaxLength(200).IsUnicode(false);
                s.HasIndex(v => v.Value).IsUnique().HasDatabaseName("UX_Products_Slug");
            });

            builder.OwnsOne(p => p.Seo, seo => {
                seo.Property(s => s.Title).HasColumnName("Seo_Title").IsRequired(false).HasMaxLength(200).IsUnicode(true);
                seo.Property(s => s.H1).HasColumnName("Seo_H1").IsRequired(false).HasMaxLength(200).IsUnicode(true);
                seo.Property(s => s.Description).HasColumnName("Seo_Description").IsRequired(false).HasMaxLength(500).IsUnicode(true);
                seo.Property(s => s.MetaDescription).HasColumnName("Seo_MetaDescription").IsRequired(false).HasMaxLength(300).IsUnicode(true);
                seo.Property(s => s.MetaKeywords).HasColumnName("Seo_MetaKeywords").IsRequired(false).HasMaxLength(300).IsUnicode(true);
            });

            // ۵. پیکربندی روابط یک‌به‌چند (مالکیت Aggregate Root با حذف آبشاری)
            builder.HasMany(p => p.Variants).WithOne().HasForeignKey(v => v.ProductId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.ProductImages).WithOne().HasForeignKey(pi => pi.ProductId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            // ۶. پیکربندی رابطه چند‌به‌چند با FeatureValue
            builder.HasMany(p => p.FeatureValues)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ProductFeatureValues",
                    right => right.HasOne<FeatureValue>().WithMany().HasForeignKey("FeatureValueId").OnDelete(DeleteBehavior.Restrict), // ✅ جلوگیری از چرخه‌ی حذف
                    left => left.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade),
                    join => {
                        join.ToTable("ProductFeatureValues", "Catalog");
                        join.HasKey("ProductId", "FeatureValueId");
                        join.HasIndex("FeatureValueId").HasDatabaseName("IX_ProductFeatureValues_FeatureValueId");
                    });

            // ۷. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، و حذف منطقی)
            ConfigureBaseEntity(builder);
        }
    }
}