using System;

namespace Application.DTOs.Identity.Person
{
    /// <summary>
    /// DTO برای نمایش اطلاعات شخص در پنل مدیریت
    /// </summary>
    public class PersonDto
    {
        /// <summary>
        /// شناسه یکتای شخص
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام شخص (اختیاری)
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی شخص (اختیاری)
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// نام کامل شخص (مقدار محاسباتی: ترکیب نام و نام خانوادگی)
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// شماره موبایل (اجباری و یکتا - کلید اصلی برای ورود)
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// شماره تلفن ثابت (اختیاری)
        /// </summary>
        public string? Landline { get; set; }

        /// <summary>
        /// آدرس ایمیل (اختیاری)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// وضعیت اتصال به حساب کاربری (آیا این شخص دارای User است؟)
        /// </summary>
        public bool HasUserAccount { get; set; }

        /// <summary>
        /// وضعیت فعلی شخص (فعال، غیرفعال یا حذف‌شده)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ و زمان ایجاد رکورد
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}