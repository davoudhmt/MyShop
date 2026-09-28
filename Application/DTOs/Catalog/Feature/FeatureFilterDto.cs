using Application.DTOs.Common;

namespace Application.DTOs.Catalog.Feature
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی ویژگی‌ها
    /// </summary>
    public class FeatureFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام ویژگی (اختیاری)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر وضعیت:
        /// - خالی یا "All": همه
        /// - "Active": فقط فعال‌ها
        /// - "Inactive": فقط غیرفعال‌ها
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// فیلتر بر اساس نوع محصول (ویژگی‌های یک نوع محصول)
        /// </summary>
        public long? ProductTypeId { get; set; }
    }
}