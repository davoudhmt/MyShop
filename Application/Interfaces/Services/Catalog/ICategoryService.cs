using Application.DTOs.Catalog.Category;
using Application.DTOs.Common;

namespace Application.Interfaces.Services.Catalog
{
    /// <summary>
    /// واسط سرویس دسته‌بندی
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// دریافت همه دسته‌بندی‌ها
        /// </summary>
        Task<List<CategoryDto>> GetAllAsync();

        /// <summary>
        /// دریافت دسته‌بندی با شناسه
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        Task<CategoryDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت دسته‌بندی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<CategoryDto>> GetPagedAsync(CategoryFilterDto filter);

        /// <summary>
        /// دریافت زیردسته‌های یک دسته
        /// </summary>
        /// <param name="parentCategoryId">شناسه دسته والد</param>
        Task<List<CategoryDto>> GetChildrenAsync(long parentCategoryId);

        /// <summary>
        /// ساخت دسته‌بندی جدید
        /// </summary>
        /// <param name="dto">اطلاعات دسته‌بندی جدید</param>
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);

        /// <summary>
        /// به‌روزرسانی دسته‌بندی
        /// </summary>
        /// <param name="dto">اطلاعات جدید دسته‌بندی</param>
        Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto);

        /// <summary>
        /// حذف منطقی دسته‌بندی
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        Task DeleteAsync(long id);

        /// <summary>
        /// فعالسازی دسته بندی
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task ActivateAsync(long id);

        /// <summary>
        /// غیرفعالسازی دسته بندی
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeactivateAsync(long id);
    }
}