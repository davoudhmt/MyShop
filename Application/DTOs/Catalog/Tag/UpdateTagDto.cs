namespace Application.DTOs.Catalog.Tag
{
    /// <summary>
    /// DTO برای به‌روزرسانی برچسب
    /// </summary>
    public class UpdateTagDto
    {
        /// <summary>
        /// شناسه برچسب (اجباری)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام برچسب (اجباری)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// وضعیت برچسب:
        /// - "Active": فعال
        /// - "Inactive": غیرفعال
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}