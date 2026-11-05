using System;

namespace Application.DTOs.Identity.User
{
    /// <summary>
    /// DTO برای نمایش اطلاعات کاربر در پنل مدیریت (Admin Panel)
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// شناسه یکتای کاربر
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// شناسه شخص مرتبط با این کاربر
        /// </summary>
        public long PersonId { get; set; }

        /// <summary>
        /// نام کاربری (شماره موبایل)
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ و زمان آخرین ورود موفق
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// آدرس IP آخرین ورود
        /// </summary>
        public string? LastLoginIp { get; set; }

        /// <summary>
        /// آیا احراز هویت دو مرحله‌ای (2FA) برای این کاربر فعال است؟
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// تعداد تلاش‌های ناموفق ورود
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// زمان پایان قفل شدن حساب (در صورت تلاش‌های ناموفق زیاد)
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// وضعیت کاربر (فعال/غیرفعال/حذف شده)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد حساب کاربری
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// تاریخ آخرین به‌روزرسانی حساب کاربری
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}