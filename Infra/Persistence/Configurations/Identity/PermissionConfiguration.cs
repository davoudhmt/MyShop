using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.Persistence.Configurations;

namespace Infra.Persistence.Configurations.Identity
{
    /// <summary>
    /// کلاس پیکربندی موجودیت مجوز (Permission) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس برای فیلدهای مجوز و روابط آن را تعریف می‌کند.
    /// </summary>
    public class PermissionConfiguration : BaseEntityConfiguration<Permission>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Permission
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Permissions", "Identity");

            // ۲. پیکربندی فیلدهای اصلی مجوز
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.Name)
                .IsUnique()
                .HasDatabaseName("UX_Permissions_Name");

            builder.Property(p => p.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Module)
                .IsRequired()
                .HasConversion<string>(); // تبدیل Enum Module به رشته در دیتابیس

            builder.HasIndex(p => p.Module)
                .HasDatabaseName("IX_Permissions_Module");

            builder.Property(p => p.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            // ۳. پیکربندی روابط ارجاعی
            // رابطه با RolePermission (یک به چند)
            builder.HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف آبشاری برای حفظ یکپارچگی داده‌ها

            // ۴. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، Status و RowVersion)
            ConfigureBaseEntity(builder);
        }
    }
}