namespace Application.DTOs.Catalog.FeatureValue
{
    /// <summary>
    /// DTO برای نمایش مقدار ویژگی
    /// </summary>
    public class FeatureValueDto
    {
        /// <summary>
        /// شناسه یکتای مقدار ویژگی
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// مقدار ویژگی
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// شناسه ویژگی
        /// </summary>
        public long FeatureId { get; set; }

        /// <summary>
        /// نام ویژگی (برای نمایش در فرانت)
        /// </summary>
        public string FeatureName { get; set; } = string.Empty;

        /// <summary>
        /// وضعیت مقدار ویژگی
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