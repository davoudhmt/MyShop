using Application.DTOs.Common;

namespace Application.DTOs.Catalog.ProductType
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی انواع محصول
    /// </summary>
    public class ProductTypeFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام نوع محصول (اختیاری)
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
        /// فیلتر بر اساس دسته‌بندی (انواع محصول یک دسته)
        /// </summary>
        public long? CategoryId { get; set; }
    }
}