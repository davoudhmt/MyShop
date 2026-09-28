namespace Application.DTOs.Catalog.FeatureValue
{
    /// <summary>
    /// DTO برای به‌روزرسانی مقدار ویژگی
    /// </summary>
    public class UpdateFeatureValueDto
    {
        /// <summary>
        /// شناسه مقدار ویژگی (اجباری)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// مقدار ویژگی (اجباری)
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// وضعیت مقدار ویژگی:
        /// - "Active": فعال
        /// - "Inactive": غیرفعال
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}