using Application.DTOs.Catalog.Product;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;

namespace Application.Interfaces.Services.Catalog
{
    /// <summary>
    /// واسط سرویس مدیریت محصول
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// دریافت محصول با شناسه (همراه با تگ‌ها، تصاویر و ویژگی‌ها)
        /// </summary>
        Task<ProductDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت محصول با اسلاگ (برای نمایش در فرانت‌اند و سئو)
        /// </summary>
        Task<ProductDto?> GetBySlugAsync(string slug);

        /// <summary>
        /// دریافت لیست محصولات با قابلیت فیلتر و صفحه‌بندی
        /// </summary>
        Task<PagedResult<ProductDto>> GetPagedAsync(ProductFilterDto filter);

        /// <summary>
        /// ایجاد محصول جدید
        /// </summary>
        Task<ProductDto> CreateAsync(CreateProductDto dto);

        /// <summary>
        /// به‌روزرسانی اطلاعات محصول
        /// </summary>
        Task<ProductDto> UpdateAsync(UpdateProductDto dto);

        /// <summary>
        /// حذف منطقی محصول (و تمام وابستگی‌های آن)
        /// </summary>
        Task DeleteAsync(long id);

        /// <summary>
        /// تنظیم یک تصویر به عنوان تصویر اصلی (کاور) محصول
        /// </summary>
        Task SetMainImageAsync(long productId, long imageId);

        /// <summary>
        /// حذف یک واریانت خاص از محصول و حذف فیزیکی تصاویر آن
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <param name="variantId">شناسه واریانت</param>
        Task RemoveVariantAsync(long productId, long variantId);
    }
}