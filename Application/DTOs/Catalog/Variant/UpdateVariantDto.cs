namespace Application.DTOs.Catalog.Variant
{
    /// <summary>
    /// DTO برای ویرایش واریانت موجود یا ساخت واریانت جدید
    /// </summary>
    public class UpdateVariantDto
    {
        /// <summary>
        /// شناسه واریانت (اگر 0 یا خالی باشد، واریانت جدید ساخته می‌شود)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// قیمت واریانت
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// درصد تخفیف (اختیاری)
        /// </summary>
        public decimal? DiscountPercent { get; set; }

        /// <summary>
        /// موجودی انبار
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// کد یکتای واریانت (SKU)
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// شناسه مقادیر ویژگی مرتبط با این واریانت
        /// </summary>
        public List<long> FeatureValueIds { get; set; } = new();

        /// <summary>
        /// آدرس تصاویر این واریانت
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();
    }
}