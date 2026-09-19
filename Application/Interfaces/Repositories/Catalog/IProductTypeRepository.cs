using Application.DTOs.Catalog.ProductType;
using Application.DTOs.Common;
using Domain.Entities.Catalog;

namespace Application.Interfaces.Repositories.Catalog
{
    /// <summary>
    /// واسط ریپازیتوری نوع محصول
    /// </summary>
    public interface IProductTypeRepository
    {
        /// <summary>
        /// دریافت همه انواع محصول
        /// </summary>
        Task<List<ProductType>> GetAllAsync();

        /// <summary>
        /// دریافت نوع محصول با شناسه
        /// </summary>
        /// <param name="id">شناسه نوع محصول</param>
        Task<ProductType?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت نوع محصول با نام
        /// </summary>
        /// <param name="name">نام نوع محصول</param>
        Task<ProductType?> GetByNameAsync(string name);

        /// <summary>
        /// دریافت انواع محصول یک دسته‌بندی خاص
        /// </summary>
        /// <param name="categoryId">شناسه دسته‌بندی</param>
        Task<List<ProductType>> GetByCategoryIdAsync(long categoryId);

        /// <summary>
        /// دریافت انواع محصول با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<ProductType>> GetPagedAsync(ProductTypeFilterDto filter);

        /// <summary>
        /// افزودن نوع محصول جدید
        /// </summary>
        /// <param name="productType">نوع محصول جدید</param>
        Task AddAsync(ProductType productType);

        /// <summary>
        /// به‌روزرسانی نوع محصول
        /// </summary>
        /// <param name="productType">نوع محصول برای به‌روزرسانی</param>
        void Update(ProductType productType);

        /// <summary>
        /// بررسی وجود نوع محصول با نام
        /// </summary>
        /// <param name="name">نام نوع محصول</param>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        Task SaveChangesAsync();
    }
}