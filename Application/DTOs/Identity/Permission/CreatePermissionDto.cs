using Domain.Enums;

namespace Application.DTOs.Identity.Permission
{
    /// <summary>
    /// DTO برای ایجاد مجوز جدید
    /// </summary>
    public class CreatePermissionDto
    {
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
    }
}