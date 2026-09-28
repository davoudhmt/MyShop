namespace Application.DTOs.Catalog.Tag
{
    /// <summary>
    /// DTO برای نمایش برچسب
    /// </summary>
    public class TagDto
    {
        /// <summary>
        /// شناسه یکتای برچسب
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام برچسب
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// وضعیت برچسب
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// تاریخ آخرین به‌روزرسانی (اختیاری)
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}