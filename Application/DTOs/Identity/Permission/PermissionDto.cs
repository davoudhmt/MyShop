using Domain.Enums;

namespace Application.DTOs.Identity.Permission
{
    /// <summary>
    /// DTO برای نمایش اطلاعات مجوز (هم در لیست و هم در جزئیات)
    /// </summary>
    public class PermissionDto
    {
        /// <summary>
        /// شناسه یکتای مجوز
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام مجوز (ساختار سلسله‌مراتبی با نقطه، مثلاً "orders.view")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// نام نمایشی مجوز (برای نمایش در پنل ادمین)
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// ماژول مجوز (برای گروه‌بندی و مدیریت بهتر)
        /// </summary>
        public Module Module { get; set; }

        /// <summary>
        /// توضیحات مجوز (اختیاری)
        /// </summary>
        public string? Description { get; set; }

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