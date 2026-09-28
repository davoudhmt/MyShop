using Application.DTOs.Catalog.Seo;

namespace Application.DTOs.Catalog.Product
{
    /// <summary>
    /// DTO برای ساخت محصول جدید
    /// </summary>
    public class CreateProductDto
    {
        /// <summary>
        /// نام محصول (اجباری)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات مختصر (اجباری)
        /// </summary>
        public string ShortDescription { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات کامل (اختیاری)
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// شناسه دسته‌بندی (اجباری)
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// شناسه نوع محصول (اجباری)
        /// </summary>
        public long ProductTypeId { get; set; }

        /// <summary>
        /// کد محصول (اجباری، ۶ کاراکتر)
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// اسلاگ URL (اجباری)
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// شناسه برند (اختیاری)
        /// </summary>
        public long? BrandId { get; set; }

        /// <summary>
        /// شناسه برچسب‌ها (اختیاری)
        /// </summary>
        public List<long> TagIds { get; set; } = new();

        /// <summary>
        /// تنظیمات سئو (اختیاری)
        /// </summary>
        public CreateSeoDto? Seo { get; set; }
    }
}