using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس پیکربندی موجودیت دسته‌بندی (Category) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس، شامل نام جدول، محدودیت‌های فیلدها، ایندکس‌های یکتا و روابط بین موجودیت‌ها را تعریف می‌کند.
    /// </summary>
    public class CategoryConfiguration : BaseEntityConfiguration<Category>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Category
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Categories", "Catalog");

            // ۲. پیکربندی فیلدهای اصلی و ایندکس‌های یکتا
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100).IsUnicode(true);
            builder.HasIndex(c => c.Name).IsUnique().HasDatabaseName("UX_Categories_Name");

            builder.Property(c => c.Description).IsRequired(false).HasMaxLength(500).IsUnicode(true);

            // ۳. پیکربندی Value Objectها (Owned Types)
            builder.OwnsOne(c => c.CategoryCode, code => {
                code.Property(cc => cc.Value).HasColumnName("CategoryCode").IsRequired().HasMaxLength(10).IsUnicode(false);
                code.HasIndex(cc => cc.Value).IsUnique().HasDatabaseName("UX_Categories_CategoryCode");
            });

            builder.OwnsOne(c => c.ImageUrl, imageUrl => {
                imageUrl.Property(i => i.Value).HasColumnName("ImageUrl").IsRequired(false).HasMaxLength(500).IsUnicode(true);
            });

            // ۴. پیکربندی رابطه بازگشتی (Self-referencing) برای دسته‌بندی والد و فرزندان
            builder.HasOne<Category>(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentCategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف آبشاری برای حفظ یکپارچگی درخت دسته‌بندی

            builder.HasIndex(c => c.ParentCategoryId).HasDatabaseName("IX_Categories_ParentCategoryId");

            // ۵. پیکربندی رابطه چند‌به‌چند با ProductType
            builder.HasMany(c => c.ProductTypes)
                .WithMany(pt => pt.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryProductTypes",
                    right => right.HasOne<ProductType>().WithMany().HasForeignKey("ProductTypeId").OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.Restrict), // ✅ جلوگیری از حذف ناخواسته دسته‌بندی
                    join => {
                        join.ToTable("CategoryProductTypes", "Catalog");
                        join.HasKey("CategoryId", "ProductTypeId");
                        join.HasIndex("ProductTypeId").HasDatabaseName("IX_CategoryProductTypes_ProductTypeId");
                    });

            // ۶. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، و حذف منطقی)
            ConfigureBaseEntity(builder);
        }
    }
}