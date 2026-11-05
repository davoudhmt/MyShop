namespace Application.DTOs.Identity.Role
{
    /// <summary>
    /// DTO برای نمایش اطلاعات نقش (هم در لیست و هم در جزئیات)
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// شناسه یکتای نقش
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام نقش (باید یونیک باشد، مثلاً "Admin", "Customer")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات نقش
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// آیا این نقش یک نقش سیستمی است؟
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// تاریخ ایجاد نقش
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// وضعیت نقش (فعال/غیرفعال/حذف شده)
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}