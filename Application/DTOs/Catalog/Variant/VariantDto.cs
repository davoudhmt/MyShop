using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Catalog.ProductImage;

namespace Application.DTOs.Catalog.Variant
{
    /// <summary>
    /// DTO برای نمایش اطلاعات واریانت
    /// </summary>
    public class VariantDto
    {
        /// <summary>
        /// شناسه یکتای واریانت
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// شناسه محصول والد
        /// </summary>
        public long ProductId { get; set; }

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
        /// مقادیر ویژگی مرتبط با این واریانت
        /// </summary>
        public List<FeatureValueDto> FeatureValues { get; set; } = new();

        /// <summary>
        /// تصاویر این واریانت
        /// </summary>
        public List<ProductImageDto> Images { get; set; } = new();
    }
}