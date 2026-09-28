namespace Application.DTOs.Catalog.Seo
{
    /// <summary>
    /// DTO برای نمایش تنظیمات سئو محصول
    /// </summary>
    public class SeoDto
    {
        /// <summary>
        /// عنوان سئو (SEO Title)
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// تگ اصلی صفحه (H1)
        /// </summary>
        public string? H1 { get; set; }

        /// <summary>
        /// توضیحات کوتاه (معمولاً برای نمایش در شبکه‌های اجتماعی)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// متا دیسکریپشن (Meta Description) برای موتورهای جستجو
        /// </summary>
        public string? MetaDescription { get; set; }

        /// <summary>
        /// کلمات کلیدی متا (Meta Keywords)
        /// </summary>
        public string? MetaKeywords { get; set; }
    }
}