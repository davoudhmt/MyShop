namespace Application.DTOs.Catalog.Seo
{
    /// <summary>
    /// DTO برای ساخت تنظیمات سئو محصول
    /// </summary>
    public class CreateSeoDto
    {
        /// <summary>
        /// عنوان سئو (اختیاری)
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// تگ اصلی صفحه (اختیاری)
        /// </summary>
        public string? H1 { get; set; }

        /// <summary>
        /// توضیحات کوتاه (اختیاری)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// متا دیسکریپشن برای موتورهای جستجو (اختیاری)
        /// </summary>
        public string? MetaDescription { get; set; }

        /// <summary>
        /// کلمات کلیدی متا (اختیاری)
        /// </summary>
        public string? MetaKeywords { get; set; }
    }
}