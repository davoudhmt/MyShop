using Domain.Entities.Location;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.Persistence.Configurations;

namespace Infra.Persistence.Configurations.Location
{
    /// <summary>
    /// کلاس پیکربندی موجودیت منطقه (Region) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس برای تقسیمات کشوری و رابطه خودارجاعی را تعریف می‌کند.
    /// </summary>
    public class RegionConfiguration : BaseEntityConfiguration<Region>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Region
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Region> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Regions", "Location");

            // ۲. پیکربندی فیلدهای اصلی منطقه
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);

            builder.HasIndex(r => r.Name)
                .HasDatabaseName("IX_Regions_Name");

            builder.Property(r => r.Type)
                .IsRequired()
                .HasConversion<string>(); // تبدیل Enum به رشته برای خوانایی بهتر در دیتابیس

            builder.Property(r => r.ParentId)
                .IsRequired(false);

            builder.HasIndex(r => r.ParentId)
                .HasDatabaseName("IX_Regions_ParentId");

            // ۳. پیکربندی رابطه خودارجاعی (Self-Referencing)
            // هر منطقه می‌تواند یک پدر (استان) و چندین فرزند (شهر) داشته باشد
            builder.HasOne(r => r.Parent)
                .WithMany(r => r.Children)
                .HasForeignKey(r => r.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف آبشاری برای حفظ یکپارچگی درخت

            // ۴. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، Status و RowVersion)
            ConfigureBaseEntity(builder);
        }
    }
}