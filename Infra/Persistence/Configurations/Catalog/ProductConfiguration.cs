using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت Product برای Entity Framework Core
    /// </summary>
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        /// <summary>
        /// متد کانفیگ جدول Product
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("Products", "Catalog");

            // ===== فیلد Name =====
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(true);

            // ایندکس روی Name برای جستجوی سریع
            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            // ===== فیلد ShortDescription =====
            builder.Property(p => p.ShortDescription)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(true);

            // ===== فیلد Description =====
            builder.Property(p => p.Description)
                .IsRequired(false)
                .HasMaxLength(2000)
                .IsUnicode(true);

            // ===== فیلد CategoryId =====
            builder.Property(p => p.CategoryId)
                .IsRequired();

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);  // دسته‌بندی با محصول حذف نمی‌شود

            builder.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            // ===== فیلد BrandId (اختیاری) =====
            builder.Property(p => p.BrandId)
                .IsRequired(false);

            builder.HasOne<Brand>()
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);  // اگه برند حذف شد، محصول بدون برند می‌مونه

            builder.HasIndex(p => p.BrandId)
                .HasDatabaseName("IX_Products_BrandId");

            // ===== فیلد ProductTypeId =====
            builder.Property(p => p.ProductTypeId)
                .IsRequired();

            builder.HasOne<ProductType>()
                .WithMany()
                .HasForeignKey(p => p.ProductTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);  // نوع محصول با محصول حذف نمی‌شود

            builder.HasIndex(p => p.ProductTypeId)
                .HasDatabaseName("IX_Products_ProductTypeId");

            // ===== Value Object: ProductCode =====
            builder.OwnsOne(p => p.ProductCode, productCode =>
            {
                productCode.Property(pc => pc.Value)
                    .HasColumnName("ProductCode")
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);  // فقط حروف و اعداد ASCII

                // ایندکس یکتا روی کد محصول (هر کد باید یکتا باشه)
                productCode.HasIndex(pc => pc.Value)
                    .IsUnique()
                    .HasDatabaseName("UX_Products_ProductCode");
            });

            // ===== Value Object: Slug =====
            builder.OwnsOne(p => p.Slug, slug =>
            {
                slug.Property(s => s.Value)
                    .HasColumnName("Slug")
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);  // فقط حروف کوچک، اعداد و خط تیره

                // ایندکس یکتا روی اسلاگ (هر اسلاگ باید یکتا باشه)
                slug.HasIndex(s => s.Value)
                    .IsUnique()
                    .HasDatabaseName("UX_Products_Slug");
            });

            // ===== Value Object: Seo (nullable) =====
            builder.OwnsOne(p => p.Seo, seo =>
            {
                seo.Property(s => s.Title)
                    .HasColumnName("Seo_Title")
                    .IsRequired(false)
                    .HasMaxLength(200)
                    .IsUnicode(true);

                seo.Property(s => s.H1)
                    .HasColumnName("Seo_H1")
                    .IsRequired(false)
                    .HasMaxLength(200)
                    .IsUnicode(true);

                seo.Property(s => s.Description)
                    .HasColumnName("Seo_Description")
                    .IsRequired(false)
                    .HasMaxLength(500)
                    .IsUnicode(true);

                seo.Property(s => s.MetaDescription)
                    .HasColumnName("Seo_MetaDescription")
                    .IsRequired(false)
                    .HasMaxLength(300)
                    .IsUnicode(true);

                seo.Property(s => s.MetaKeywords)
                    .HasColumnName("Seo_MetaKeywords")
                    .IsRequired(false)
                    .HasMaxLength(300)
                    .IsUnicode(true);
            });

            // ===== رابطه یک‌به‌چند با Variant =====
            builder.HasMany(p => p.Variants)
                .WithOne()
                .HasForeignKey(v => v.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);  // با حذف محصول، واریانت‌ها هم حذف می‌شن

            // ===== رابطه یک‌به‌چند با ProductImage =====
            builder.HasMany(p => p.ProductImages)
                .WithOne()
                .HasForeignKey(pi => pi.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);  // با حذف محصول، تصاویر هم حذف می‌شن

            // ===== رابطه چند‌به‌چند با FeatureValue (ویژگی‌های یگانه/توضیحی) =====
            builder.HasMany(p => p.FeatureValues)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ProductFeatureValues",
                    right => right.HasOne<FeatureValue>()
                        .WithMany()
                        .HasForeignKey("FeatureValueId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("ProductFeatureValues", "catalog");
                        join.HasKey("ProductId", "FeatureValueId");
                        join.HasIndex("FeatureValueId")
                            .HasDatabaseName("IX_ProductFeatureValues_FeatureValueId");
                    });

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}