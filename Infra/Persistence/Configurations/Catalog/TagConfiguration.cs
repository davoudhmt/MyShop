using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    /// <summary>
    /// کلاس پیکربندی موجودیت برچسب (Tag) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس، شامل نام جدول، محدودیت‌های فیلدها، ایندکس‌های یکتا و رابطه چندبه‌چند با محصول را تعریف می‌کند.
    /// </summary>
    public class TagConfiguration : BaseEntityConfiguration<Tag>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Tag
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Tags", "Catalog");

            // ۲. پیکربندی فیلدهای اصلی و ایندکس یکتا
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50).IsUnicode(true);
            builder.HasIndex(t => t.Name).IsUnique().HasDatabaseName("UX_Tags_Name");

            // ۳. پیکربندی رابطه چند‌به‌چند با Product
            builder.HasMany(t => t.Products)
                .WithMany(p => p.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductTags",
                    right => right.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Restrict), // ✅ جلوگیری از حذف ناخواسته برچسب هنگام حذف محصول
                    join => {
                        join.ToTable("ProductTags", "Catalog");
                        join.HasKey("ProductId", "TagId");
                        join.HasIndex("TagId").HasDatabaseName("IX_ProductTags_TagId");
                    });

            // ۴. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، و حذف منطقی)
            ConfigureBaseEntity(builder);
        }
    }
}