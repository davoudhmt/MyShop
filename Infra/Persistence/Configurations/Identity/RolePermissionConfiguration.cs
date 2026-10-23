using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.Persistence.Configurations;

namespace Infra.Persistence.Configurations.Identity
{
    /// <summary>
    /// کلاس پیکربندی موجودیت ارتباطی مجوز نقش (RolePermission) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس برای جدول واسط بین Role و Permission را تعریف می‌کند.
    /// </summary>
    public class RolePermissionConfiguration : BaseEntityConfiguration<RolePermission>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت RolePermission
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("RolePermissions", "Identity");

            // ۲. پیکربندی ایندکس ترکیبی یونیک
            // جلوگیری از تخصیص تکراری یک مجوز به یک نقش
            // (رابطه اصلی در RoleConfiguration و PermissionConfiguration تعریف شده است)
            builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique()
                .HasDatabaseName("UX_RolePermissions_RoleId_PermissionId");

            // ۳. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، Status و RowVersion)
            ConfigureBaseEntity(builder);
        }
    }
}