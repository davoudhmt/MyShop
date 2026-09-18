namespace Application.DTOs.Catalog.Category
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی دسته‌بندی‌ها
    /// </summary>
    public class CategoryFilterDto
    {
        /// <summary>
        /// عبارت جستجو در نام دسته‌بندی (اختیاری)
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
        /// فیلتر بر اساس پدر (برای دیدن زیردسته‌ها)
        /// </summary>
        public long? ParentCategoryId { get; set; }

        /// <summary>
        /// فقط ریشه‌ها (برای نمایش منوی اصلی)
        /// </summary>
        public bool OnlyRoots { get; set; } = false;

        /// <summary>
        /// شماره صفحه (از ۱ شروع می‌شه)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// تعداد آیتم در هر صفحه
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}