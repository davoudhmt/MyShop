namespace Application.DTOs.Identity.Role
{
    /// <summary>
    /// DTO برای به‌روزرسانی نقش موجود
    /// </summary>
    public class UpdateRoleDto
    {
        /// <summary>
        /// شناسه نقش (برای به‌روزرسانی)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام جدید نقش
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات جدید نقش
        /// </summary>
        public string? Description { get; set; }

        // نکته: IsSystem معمولاً پس از ایجاد تغییر نمی‌کند، بنابراین در DTO به‌روزرسانی قرار نگرفت.
    }
}