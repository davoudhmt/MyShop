using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.Persistence.Configurations;

namespace Infra.Persistence.Configurations.Identity
{
    /// <summary>
    /// کلاس پیکربندی موجودیت ارتباطی نقش کاربر (UserRole) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس برای جدول واسط بین User و Role را تعریف می‌کند.
    /// </summary>
    public class UserRoleConfiguration : BaseEntityConfiguration<UserRole>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت UserRole
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("UserRoles", "Identity");

            // ۲. پیکربندی ایندکس ترکیبی یونیک
            // جلوگیری از تخصیص تکراری یک نقش به یک کاربر
            // (رابطه اصلی در UserConfiguration تعریف شده است)
            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique()
                .HasDatabaseName("UX_UserRoles_UserId_RoleId");

            // ۳. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، Status و RowVersion)
            ConfigureBaseEntity(builder);
        }
    }
}