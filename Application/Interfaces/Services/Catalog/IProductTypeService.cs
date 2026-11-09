using Application.DTOs.Catalog.ProductType;
using Application.DTOs.Common;

namespace Application.Interfaces.Services.Catalog
{
    /// <summary>
    /// واسط سرویس نوع محصول
    /// </summary>
    public interface IProductTypeService
    {
        /// <summary>
        /// دریافت همه انواع محصول
        /// </summary>
        Task<List<ProductTypeDto>> GetAllAsync();

        /// <summary>
        /// دریافت نوع محصول با شناسه
        /// </summary>
        /// <param name="id">شناسه نوع محصول</param>
        Task<ProductTypeDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت انواع محصول یک دسته‌بندی خاص
        /// </summary>
        /// <param name="categoryId">شناسه دسته‌بندی</param>
        Task<List<ProductTypeDto>> GetByCategoryIdAsync(long categoryId);

        /// <summary>
        /// دریافت انواع محصول با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<ProductTypeDto>> GetPagedAsync(ProductTypeFilterDto filter);

        /// <summary>
        /// ساخت نوع محصول جدید
        /// </summary>
        /// <param name="dto">اطلاعات نوع محصول جدید</param>
        Task<ProductTypeDto> CreateAsync(CreateProductTypeDto dto);

        /// <summary>
        /// به‌روزرسانی نوع محصول
        /// </summary>
        /// <param name="dto">اطلاعات جدید نوع محصول</param>
        Task<ProductTypeDto> UpdateAsync(UpdateProductTypeDto dto);

        /// <summary>
        /// حذف منطقی نوع محصول
        /// </summary>
        /// <param name="id">شناسه نوع محصول</param>
        Task DeleteAsync(long id);

        /// <summary>
        /// فعالسازی نوع محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task ActivateAsync(long id);

        /// <summary>
        /// غیرفعالسازی نوع محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeactivateAsync(long id);
    }
}