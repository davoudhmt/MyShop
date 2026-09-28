namespace Application.DTOs.Catalog.ProductImage
{
    /// <summary>
    /// DTO برای نمایش اطلاعات تصویر محصول
    /// </summary>
    public class ProductImageDto
    {
        /// <summary>
        /// شناسه یکتای تصویر
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// آدرس (URL) تصویر
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// شناسه محصول مرتبط
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// شناسه واریانت مرتبط (اگر خالی باشد، یعنی تصویر متعلق به گالری اصلی محصول است)
        /// </summary>
        public long? VariantId { get; set; }

        /// <summary>
        /// تعیین می‌کند آیا این تصویر اصلی (کاور) است یا خیر
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// متن جایگزین تصویر (Alt Text) برای سئو و دسترسی‌پذیری
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// وضعیت تصویر (Active, Inactive, Deleted)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// تاریخ آخرین به‌روزرسانی (اختیاری)
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}