using Application.DTOs.Catalog.Seo;
using Application.DTOs.Catalog.Variant;

namespace Application.DTOs.Catalog.Product
{
    /// <summary>
    /// DTO برای به‌روزرسانی محصول
    /// </summary>
    public class UpdateProductDto
    {
        /// <summary>
        /// شناسه محصول (اجباری)
        /// </summary>
        public long Id { get; set; }

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
        /// کد محصول (اجباری)
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
        /// شناسه برچسب‌ها (جایگزین لیست قبلی می‌شود)
        /// </summary>
        public List<long> TagIds { get; set; } = new();

        /// <summary>
        /// تنظیمات سئو (اختیاری)
        /// </summary>
        public UpdateSeoDto? Seo { get; set; }

        /// <summary>
        /// لیست واریانت‌های محصول (برای ویرایش واریانت‌های موجود و ساخت واریانت‌های جدید)
        /// واریانت‌هایی که در این لیست نباشند، حذف خواهند شد
        /// </summary>
        public List<UpdateVariantDto> Variants { get; set; } = new();
    }
}