using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت Variant برای Entity Framework Core
    /// </summary>
    public class VariantConfiguration : BaseEntityConfiguration<Variant>
    {
        /// <summary>
        /// متد کانفیگ جدول Variant
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<Variant> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("Variants");

            // ===== فیلد ProductId =====
            builder.Property(v => v.ProductId)
                .IsRequired();

            builder.HasOne<Product>()
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(v => v.ProductId)
                .HasDatabaseName("IX_Variants_ProductId");

            // ===== فیلد Price =====
            builder.Property(v => v.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // ===== فیلد DiscountPercent =====
            builder.Property(v => v.DiscountPercent)
                .IsRequired(false)
                .HasColumnType("decimal(5,2)");

            // ===== فیلد Stock =====
            builder.Property(v => v.Stock)
                .IsRequired();

            // ===== Value Object: SKU =====
            builder.OwnsOne(v => v.SKU, sku =>
            {
                sku.Property(s => s.Value)
                    .HasColumnName("SKU")
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);  // فقط حروف و اعداد ASCII

                // ایندکس یکتا روی SKU (هر SKU باید یکتا باشه)
                sku.HasIndex(s => s.Value)
                    .IsUnique()
                    .HasDatabaseName("UX_Variants_SKU");
            });

            // ===== رابطه یک‌به‌چند با ProductImage =====
            builder.HasMany(v => v.ProductImages)
                .WithOne()
                .HasForeignKey("VariantId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);  // اگه واریانت حذف شد، تصاویر به گالری اصلی برمی‌گردن

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}