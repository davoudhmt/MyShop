namespace Application.DTOs.Identity.Role
{
    /// <summary>
    /// DTO برای ایجاد نقش جدید
    /// </summary>
    public class CreateRoleDto
    {
        /// <summary>
        /// نام نقش (باید یونیک باشد)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات نقش (برای نقش‌های غیرسیستمی اجباری است)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// آیا این نقش یک نقش سیستمی است؟ (پیش‌فرض: false)
        /// </summary>
        public bool IsSystem { get; set; } = false;
    }
}