using Application.DTOs.Common;

namespace Application.DTOs.Catalog.Category
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی دسته‌بندی‌ها
    /// </summary>
    public class CategoryFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام دسته‌بندی (اختیاری)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر وضعیت
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// فیلتر بر اساس پدر (برای دیدن زیردسته‌ها)
        /// </summary>
        public long? ParentCategoryId { get; set; }

        /// <summary>
        /// فقط ریشه‌ها (برای نمایش منوی اصلی)
        /// </summary>
        public bool OnlyRoots { get; set; } = false;
    }
}