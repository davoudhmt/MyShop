using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت ProductImage برای Entity Framework Core
    /// </summary>
    public class ProductImageConfiguration : BaseEntityConfiguration<ProductImage>
    {
        /// <summary>
        /// متد کانفیگ جدول ProductImage
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("ProductImages", "Catalog");

            // ===== فیلد ProductId =====
            builder.Property(pi => pi.ProductId)
                .IsRequired();

            builder.HasOne<Product>()
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);   // جلوگیری از چرخه حذف (مدیریت حذف توسط Aggregate Root انجام می‌شود)

            builder.HasIndex(pi => pi.ProductId)
                .HasDatabaseName("IX_ProductImages_ProductId");

            // ===== فیلد VariantId =====
            builder.Property(pi => pi.VariantId)
                .IsRequired(false);

            // رابطه با Variant (چون در VariantConfiguration قبلاً تعریف شده، اینجا فقط جزئیات)
            builder.HasOne<Variant>()
                .WithMany(v => v.ProductImages)
                .HasForeignKey(pi => pi.VariantId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);  // با حذف واریانت، تصویر به گالری اصلی برمی‌گرده

            builder.HasIndex(pi => pi.VariantId)
                .HasDatabaseName("IX_ProductImages_VariantId");

            // ===== ایندکس یکتا برای قانون «فقط یک تصویر اصلی» =====
            // برای هر ترکیب (ProductId, VariantId)، فقط یک تصویر می‌تونه IsMain = true باشه
            builder.HasIndex(pi => new { pi.ProductId, pi.VariantId })
                .IsUnique()
                .HasFilter("[IsMain] = 1")  // فقط تصاویر اصلی ایندکس می‌شن
                .HasDatabaseName("UX_ProductImages_MainPerProductVariant");

            // ===== فیلد IsMain =====
            builder.Property(pi => pi.IsMain)
                .IsRequired()
                .HasDefaultValue(false);

            // ===== فیلد AltText =====
            builder.Property(pi => pi.AltText)
                .IsRequired(false)
                .HasMaxLength(250)
                .IsUnicode(true);

            // ===== Value Object: ImageUrl =====
            builder.OwnsOne(pi => pi.ImageUrl, imageUrl =>
            {
                imageUrl.Property(i => i.Value)
                    .HasColumnName("ImageUrl")
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(true);
            });

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}