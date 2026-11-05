namespace Application.DTOs.Identity.User
{
    /// <summary>
    /// DTO برای نمایش اطلاعات پروفایل کاربر جاری
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// شناسه شخص
        /// </summary>
        public long PersonId { get; set; }

        /// <summary>
        /// نام شخص
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی شخص
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// آدرس ایمیل (اختیاری)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// وضعیت برند
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// تاریخ به‌روزرسانی
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}