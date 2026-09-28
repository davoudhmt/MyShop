using Application.DTOs.Common;

namespace Application.DTOs.Catalog.FeatureValue
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی مقادیر ویژگی
    /// </summary>
    public class FeatureValueFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در مقدار ویژگی (اختیاری)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر بر اساس ویژگی (اختیاری)
        /// </summary>
        public long? FeatureId { get; set; }

        /// <summary>
        /// فیلتر وضعیت:
        /// - خالی یا "All": همه
        /// - "Active": فقط فعال‌ها
        /// - "Inactive": فقط غیرفعال‌ها
        /// </summary>
        public string? Status { get; set; }
    }
}