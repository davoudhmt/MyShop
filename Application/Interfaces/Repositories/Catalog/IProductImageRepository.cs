using Domain.Entities.Catalog;

namespace Application.Interfaces.Repositories.Catalog
{
    /// <summary>
    /// واسط ریپازیتوری تصویر محصول
    /// </summary>
    public interface IProductImageRepository
    {
        /// <summary>
        /// دریافت تصویر با شناسه
        /// </summary>
        /// <param name="id">شناسه تصویر</param>
        Task<ProductImage?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت همه تصاویر یک محصول (شامل گالری اصلی و واریانت‌ها)
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        Task<List<ProductImage>> GetByProductIdAsync(long productId);

        /// <summary>
        /// دریافت تصویر اصلی (کاور) یک محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        Task<ProductImage?> GetMainImageByProductIdAsync(long productId);

        /// <summary>
        /// دریافت تصاویر یک واریانت خاص
        /// </summary>
        /// <param name="variantId">شناسه واریانت</param>
        Task<List<ProductImage>> GetByVariantIdAsync(long variantId);

        /// <summary>
        /// افزودن تصویر جدید
        /// </summary>
        /// <param name="image">تصویر جدید</param>
        Task AddAsync(ProductImage image);

        /// <summary>
        /// به‌روزرسانی تصویر (برای تغییرات و همچنین حذف منطقی)
        /// </summary>
        /// <param name="image">تصویر برای به‌روزرسانی</param>
        void Update(ProductImage image);

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        Task SaveChangesAsync();
    }
}