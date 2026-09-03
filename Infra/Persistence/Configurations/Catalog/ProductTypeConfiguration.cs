using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت ProductType برای Entity Framework Core
    /// </summary>
    public class ProductTypeConfiguration : BaseEntityConfiguration<ProductType>
    {
        /// <summary>
        /// متد کانفیگ جدول ProductType
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<ProductType> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("ProductTypes");

            // ===== فیلد Name =====
            builder.Property(pt => pt.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);

            // ایندکس یکتا روی Name
            builder.HasIndex(pt => pt.Name)
                .IsUnique()
                .HasDatabaseName("UX_ProductTypes_Name");

            // ===== فیلد Description =====
            builder.Property(pt => pt.Description)
                .IsRequired(false)
                .HasMaxLength(500)
                .IsUnicode(true);

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}