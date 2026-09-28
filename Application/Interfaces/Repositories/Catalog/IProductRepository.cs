using Application.DTOs.Catalog.Product;
using Application.DTOs.Common;
using Domain.Entities.Catalog;

namespace Application.Interfaces.Repositories.Catalog
{
    /// <summary>
    /// واسط ریپازیتوری محصول
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// دریافت محصول با شناسه (همراه با داده‌های مرتبط مثل تگ‌ها و تصاویر)
        /// </summary>
        Task<Product?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت محصولات با صفحه‌بندی و فیلتر
        /// </summary>
        Task<PagedResult<Product>> GetPagedAsync(ProductFilterDto filter);

        /// <summary>
        /// دریافت محصول با اسلاگ (آدرس URL)
        /// </summary>
        Task<Product?> GetBySlugAsync(string slug);

        /// <summary>
        /// بررسی وجود محصول با کد مشخص
        /// </summary>
        Task<bool> ExistsByCodeAsync(string productCode);

        /// <summary>
        /// بررسی وجود محصول با اسلاگ مشخص
        /// </summary>
        Task<bool> ExistsBySlugAsync(string slug);

        /// <summary>
        /// افزودن محصول جدید
        /// </summary>
        Task AddAsync(Product product);

        /// <summary>
        /// به‌روزرسانی محصول
        /// </summary>
        void Update(Product product);

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        Task SaveChangesAsync();
    }
}