using Application.DTOs.Common;

namespace Application.DTOs.Catalog.Product
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی محصولات
    /// </summary>
    public class ProductFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام یا کد محصول (اختیاری)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر بر اساس شناسه دسته‌بندی (اختیاری)
        /// </summary>
        public long? CategoryId { get; set; }

        /// <summary>
        /// فیلتر بر اساس شناسه برند (اختیاری)
        /// </summary>
        public long? BrandId { get; set; }

        /// <summary>
        /// فیلتر بر اساس شناسه نوع محصول (اختیاری)
        /// </summary>
        public long? ProductTypeId { get; set; }

        /// <summary>
        /// فیلتر وضعیت:
        /// - خالی یا "All": همه
        /// - "Active": فقط فعال‌ها
        /// - "Inactive": فقط غیرفعال‌ها
        /// </summary>
        public string? Status { get; set; }
    }
}