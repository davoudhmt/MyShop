using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت Category برای Entity Framework Core
    /// </summary>
    public class CategoryConfiguration : BaseEntityConfiguration<Category>
    {
        /// <summary>
        /// متد کانفیگ جدول Category
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("Categories", "Catalog");

            // ===== فیلد Name =====
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);

            // ایندکس یکتا روی Name (نام برند باید یکتا باشه)
            builder.HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("UX_Categories_Name");

            // ===== فیلد Description =====
            builder.Property(c => c.Description)
                .IsRequired(false)
                .HasMaxLength(500)
                .IsUnicode(true);

            // ===== Value Object: CategoryCode =====
            builder.OwnsOne(c => c.CategoryCode, code =>
            {
                code.Property(cc => cc.Value)
                    .HasColumnName("CategoryCode")
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                // ایندکس یکتا روی کد دسته‌بندی
                code.HasIndex(cc => cc.Value)
                    .IsUnique()
                    .HasDatabaseName("UX_Categories_CategoryCode");
            });

            // ===== Value Object: ImageUrl =====
            builder.OwnsOne(c => c.ImageUrl, imageUrl =>
            {
                imageUrl.Property(i => i.Value)
                    .HasColumnName("ImageUrl")
                    .IsRequired(false)
                    .HasMaxLength(500)
                    .IsUnicode(true);
            });

            // ===== رابطه سلسله‌مراتبی (Self-referencing) =====
            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(c => c.ParentCategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // ایندکس روی ParentCategoryId برای کوئری‌های سریع
            builder.HasIndex(c => c.ParentCategoryId)
                .HasDatabaseName("IX_Categories_ParentCategoryId");

            // ===== رابطه چند‌به‌چند با ProductType =====
            builder.HasMany(c => c.ProductTypes)
                .WithMany(pt => pt.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryProductTypes",
                    right => right.HasOne<ProductType>()
                        .WithMany()
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("CategoryProductTypes", "Catalog");
                        join.HasKey("CategoryId", "ProductTypeId");
                        join.HasIndex("ProductTypeId")
                            .HasDatabaseName("IX_CategoryProductTypes_ProductTypeId");
                    });

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}
