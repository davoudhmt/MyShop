using Application.DTOs.Common;

namespace Application.DTOs.Catalog.Tag
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی برچسب‌ها
    /// </summary>
    public class TagFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام برچسب (اختیاری)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر وضعیت:
        /// - خالی یا "All": همه
        /// - "Active": فقط فعال‌ها
        /// - "Inactive": فقط غیرفعال‌ها
        /// </summary>
        public string? Status { get; set; }
    }
}