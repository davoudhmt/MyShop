using Application.DTOs.Catalog.ProductImage;

namespace Application.Interfaces.Services.Catalog
{
    /// <summary>
    /// واسط سرویس تصویر محصول
    /// </summary>
    public interface IProductImageService
    {
        /// <summary>
        /// دریافت تصویر با شناسه
        /// </summary>
        /// <param name="id">شناسه تصویر</param>
        Task<ProductImageDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت همه تصاویر یک محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        Task<List<ProductImageDto>> GetByProductIdAsync(long productId);

        /// <summary>
        /// دریافت تصویر اصلی (کاور) یک محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        Task<ProductImageDto?> GetMainImageByProductIdAsync(long productId);

        /// <summary>
        /// دریافت تصاویر یک واریانت خاص
        /// </summary>
        /// <param name="variantId">شناسه واریانت</param>
        Task<List<ProductImageDto>> GetByVariantIdAsync(long variantId);

        /// <summary>
        /// ساخت تصویر جدید
        /// </summary>
        /// <param name="dto">اطلاعات تصویر جدید</param>
        Task<ProductImageDto> CreateAsync(CreateProductImageDto dto);

        /// <summary>
        /// به‌روزرسانی اطلاعات تصویر
        /// </summary>
        /// <param name="dto">اطلاعات جدید تصویر</param>
        Task<ProductImageDto> UpdateAsync(UpdateProductImageDto dto);

        /// <summary>
        /// حذف منطقی تصویر
        /// </summary>
        /// <param name="id">شناسه تصویر</param>
        Task DeleteAsync(long id);

        /// <summary>
        /// فعالسازی تصویر
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task ActivateAsync(long id);

        /// <summary>
        /// غیرفعالسازی تصویر
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeactivateAsync(long id);
    }
}