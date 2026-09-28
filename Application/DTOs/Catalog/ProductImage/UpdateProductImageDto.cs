namespace Application.DTOs.Catalog.ProductImage
{
    /// <summary>
    /// DTO برای به‌روزرسانی اطلاعات تصویر محصول
    /// </summary>
    public class UpdateProductImageDto
    {
        /// <summary>
        /// شناسه یکتای تصویر (اجباری)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// آدرس (URL) جدید تصویر (اختیاری)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// آیا این تصویر اصلی (کاور) است؟ (اختیاری)
        /// </summary>
        public bool? IsMain { get; set; }

        /// <summary>
        /// متن جایگزین تصویر جدید برای سئو (اختیاری)
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// وضعیت جدید تصویر:
        /// - "Active": فعال
        /// - "Inactive": غیرفعال
        /// </summary>
        public string? Status { get; set; }
    }
}