using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس کانفیگ موجودیت Tag برای Entity Framework Core
    /// </summary>
    public class TagConfiguration : BaseEntityConfiguration<Tag>
    {
        /// <summary>
        /// متد کانفیگ جدول Tag
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            // ===== نام جدول =====
            builder.ToTable("Tags", "Catalog");

            // ===== فیلد Name =====
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(true);

            // ایندکس یکتا روی Name (نام تگ باید یکتا باشه)
            builder.HasIndex(t => t.Name)
                .IsUnique()
                .HasDatabaseName("UX_Tags_Name");

            // ===== رابطه چند‌به‌چند با Product =====
            builder.HasMany(t => t.Products)
                .WithMany(p => p.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductTags",
                    right => right.HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Tag>()
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("ProductTags", "Catalog");
                        join.HasKey("ProductId", "TagId");
                        join.HasIndex("TagId")
                            .HasDatabaseName("IX_ProductTags_TagId");
                    });

            // ===== کانفیگ فیلدهای مشترک BaseEntity =====
            ConfigureBaseEntity(builder);
        }
    }
}