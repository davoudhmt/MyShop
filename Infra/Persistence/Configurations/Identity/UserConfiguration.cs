using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.Persistence.Configurations;

namespace Infra.Persistence.Configurations.Identity
{
    /// <summary>
    /// کلاس پیکربندی موجودیت کاربر (User) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس برای فیلدهای احراز هویت، امنیت و روابط را تعریف می‌کند.
    /// </summary>
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت User
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Users", "Identity");

            // ۲. پیکربندی فیلدهای اصلی احراز هویت
            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(11) // فرمت موبایل ایران
                .IsUnicode(false); // فقط اعداد

            builder.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("UX_Users_Username");

            builder.Property(u => u.PasswordHash)
                .IsRequired(false) // برای کاربرانی که فقط با OTP وارد می‌شوند
                .HasMaxLength(256); // طول استاندارد برای هش‌های مدرن

            // ۳. پیکربندی فیلدهای امنیتی و قفل شدن
            builder.Property(u => u.AccessFailedCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(u => u.LockoutEnd)
                .IsRequired(false);

            builder.Property(u => u.SecurityStamp)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.ConcurrencyStamp)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.TwoFactorEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            // ۴. پیکربندی فیلدهای ردیابی ورود و توکن
            builder.Property(u => u.LastLoginDate)
                .IsRequired(false);

            builder.Property(u => u.LastLoginIp)
                .IsRequired(false)
                .HasMaxLength(45); // حداکثر طول آدرس IPv6

            builder.Property(u => u.RefreshToken)
                .IsRequired(false)
                .HasMaxLength(512);

            builder.Property(u => u.RefreshTokenExpiryTime)
                .IsRequired(false);

            // ۵. پیکربندی روابط ارجاعی
            // رابطه با Person (یک به یک) - کلید خارجی در همین جدول است
            builder.Property(u => u.PersonId)
                .IsRequired();

            builder.HasIndex(u => u.PersonId)
                .IsUnique()
                .HasDatabaseName("UX_Users_PersonId");

            // رابطه با UserRole (یک به چند)
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف آبشاری نقش‌ها هنگام حذف کاربر

            // ۶. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، Status و RowVersion)
            ConfigureBaseEntity(builder);
        }
    }
}