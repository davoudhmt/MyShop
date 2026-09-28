using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Catalog.ProductImage;
using Application.DTOs.Catalog.Seo;
using Application.DTOs.Catalog.Tag;

namespace Application.DTOs.Catalog.Product
{
    /// <summary>
    /// DTO برای نمایش کامل اطلاعات محصول
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// شناسه یکتای محصول
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام محصول
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات مختصر
        /// </summary>
        public string ShortDescription { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات کامل
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// کد محصول
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// اسلاگ (آدرس URL)
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// شناسه دسته‌بندی
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// شناسه برند (اختیاری)
        /// </summary>
        public long? BrandId { get; set; }

        /// <summary>
        /// شناسه نوع محصول
        /// </summary>
        public long ProductTypeId { get; set; }

        /// <summary>
        /// وضعیت محصول
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// تاریخ آخرین به‌روزرسانی
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        // ===== لیست‌های مرتبط =====

        /// <summary>
        /// برچسب‌های محصول
        /// </summary>
        public List<TagDto> Tags { get; set; } = new();

        /// <summary>
        /// مقادیر ویژگی‌های یگانه
        /// </summary>
        public List<FeatureValueDto> FeatureValues { get; set; } = new();

        /// <summary>
        /// تصاویر محصول
        /// </summary>
        public List<ProductImageDto> Images { get; set; } = new();

        /// <summary>
        /// تنظیمات سئو محصول (اختیاری)
        /// </summary>
        public SeoDto? Seo { get; set; }
    }
}