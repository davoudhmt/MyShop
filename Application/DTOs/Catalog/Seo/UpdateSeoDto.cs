namespace Application.DTOs.Catalog.Seo
{
    /// <summary>
    /// DTO برای به‌روزرسانی تنظیمات سئو محصول
    /// </summary>
    public class UpdateSeoDto
    {
        /// <summary>
        /// عنوان سئو جدید (اختیاری)
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// تگ اصلی صفحه جدید (اختیاری)
        /// </summary>
        public string? H1 { get; set; }

        /// <summary>
        /// توضیحات کوتاه جدید (اختیاری)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// متا دیسکریپشن جدید برای موتورهای جستجو (اختیاری)
        /// </summary>
        public string? MetaDescription { get; set; }

        /// <summary>
        /// کلمات کلیدی متا جدید (اختیاری)
        /// </summary>
        public string? MetaKeywords { get; set; }
    }
}