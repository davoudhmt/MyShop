namespace Application.DTOs.Catalog.Feature
{
    /// <summary>
    /// DTO برای نمایش ویژگی
    /// </summary>
    public class FeatureDto
    {
        /// <summary>
        /// شناسه یکتای ویژگی
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام ویژگی
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// چندگانگی ویژگی (Single یا Multiple)
        /// </summary>
        public string Multiplicity { get; set; } = string.Empty;

        /// <summary>
        /// وضعیت ویژگی
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