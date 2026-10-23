using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.Persistence.Configurations;

namespace Infra.Persistence.Configurations.Identity
{
    /// <summary>
    /// کلاس پیکربندی موجودیت نقش (Role) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس برای فیلدهای نقش و روابط آن را تعریف می‌کند.
    /// </summary>
    public class RoleConfiguration : BaseEntityConfiguration<Role>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Role
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Roles", "Identity");

            // ۲. پیکربندی فیلدهای اصلی نقش
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.Name)
                .IsUnique()
                .HasDatabaseName("UX_Roles_Name");

            builder.Property(r => r.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(r => r.IsSystem)
                .IsRequired()
                .HasDefaultValue(false);

            // ۳. پیکربندی روابط ارجاعی
            // رابطه با RolePermission (یک به چند)
            builder.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف آبشاری برای حفظ یکپارچگی داده‌ها

            // ۴. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، Status و RowVersion)
            ConfigureBaseEntity(builder);
        }
    }
}