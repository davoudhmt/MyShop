namespace Application.DTOs.Catalog.Brand
{
    /// <summary>
    /// DTO برای فیلتر و جستجوی برندها
    /// </summary>
    public class BrandFilterDto
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