namespace Application.DTOs.Catalog.ProductImage
{
    /// <summary>
    /// DTO برای ساخت تصویر جدید برای محصول یا واریانت
    /// </summary>
    public class CreateProductImageDto
    {
        /// <summary>
        /// آدرس (URL) تصویر (اجباری)
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// شناسه محصول مرتبط (اجباری)
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// شناسه واریانت مرتبط (اختیاری - اگر خالی باشد، تصویر به گالری اصلی محصول تعلق دارد)
        /// </summary>
        public long? VariantId { get; set; }

        /// <summary>
        /// آیا این تصویر اصلی (کاور) است؟ (پیش‌فرض: خیر)
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// متن جایگزین تصویر برای سئو (اختیاری، حداکثر ۲۵۰ کاراکتر)
        /// </summary>
        public string? AltText { get; set; }
    }
}