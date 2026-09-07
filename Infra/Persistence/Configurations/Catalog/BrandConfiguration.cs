using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت Brand برای Entity Framework Core
    /// </summary>
    public class BrandConfiguration : BaseEntityConfiguration<Brand>
    {
        /// <summary>
        /// متد کانفیگ جدول Brand
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<Brand> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("Brands", "Catalog");

            // ===== فیلد Name =====
            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);

            // ایندکس یکتا روی Name (نام برند باید یکتا باشه)
            builder.HasIndex(b => b.Name)
                .IsUnique()
                .HasDatabaseName("UX_Brands_Name");

            // ===== فیلد Description =====
            builder.Property(b => b.Description)
                .IsRequired(false)
                .HasMaxLength(500)
                .IsUnicode(true);

            // ===== Value Object: ImageUrl =====
            builder.OwnsOne(b => b.ImageUrl, imageUrl =>
            {
                imageUrl.Property(i => i.Value)
                    .HasColumnName("ImageUrl")
                    .IsRequired(false)
                    .HasMaxLength(500)
                    .IsUnicode(true);
            });

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}