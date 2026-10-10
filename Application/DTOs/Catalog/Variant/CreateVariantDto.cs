namespace Application.DTOs.Catalog.Variant
{
    /// <summary>
    /// DTO برای ساخت یک واریانت جدید درون محصول
    /// </summary>
    public class CreateVariantDto
    {
        /// <summary>
        /// قیمت واریانت
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// درصد تخفیف (اختیاری، بین 0 تا 100)
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
        /// شناسه مقادیر ویژگی مرتبط با این واریانت (اختیاری)
        /// مثال: شناسه ویژگی "رنگ: قرمز" و "سایز: L"
        /// </summary>
        public List<long> FeatureValueIds { get; set; } = new();

        /// <summary>
        /// آدرس تصاویر این واریانت (اختیاری)
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();
    }
}