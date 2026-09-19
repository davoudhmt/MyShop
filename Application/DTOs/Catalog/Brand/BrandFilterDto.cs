using Application.DTOs.Common;

namespace Application.DTOs.Catalog.Brand
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی برندها
    /// </summary>
    public class BrandFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام برند (اختیاری)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر وضعیت:
        /// - خالی یا "All": همه برندها
        /// - "Active": فقط فعال‌ها
        /// - "Inactive": فقط غیرفعال‌ها
        /// </summary>
        public string? Status { get; set; }
    }
}