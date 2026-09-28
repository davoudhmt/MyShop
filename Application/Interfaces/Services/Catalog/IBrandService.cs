using Application.DTOs.Catalog.Brand;
using Application.DTOs.Common;

namespace Application.Interfaces.Services.Catalog
{
    /// <summary>
    /// واسط سرویس برند
    /// </summary>
    public interface IBrandService
    {
        /// <summary>
        /// دریافت همه برندها
        /// </summary>
        /// <returns>لیست برندها</returns>
        Task<List<BrandDto>> GetAllAsync();

        /// <summary>
        /// دریافت برندها با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        /// <returns>نتیجه صفحه‌بندی شده</returns>
        Task<PagedResult<BrandDto>> GetPagedAsync(BrandFilterDto filter);

        /// <summary>
        /// دریافت برند با شناسه
        /// </summary>
        /// <param name="id">شناسه برند</param>
        /// <returns>برند یا null</returns>
        Task<BrandDto?> GetByIdAsync(long id);

        /// <summary>
        /// ساخت برند جدید
        /// </summary>
        /// <param name="dto">اطلاعات برند جدید</param>
        /// <returns>شناسه برند ساخته‌شده</returns>
        Task<BrandDto> CreateAsync(CreateBrandDto dto);

        /// <summary>
        /// به‌روزرسانی برند
        /// </summary>
        /// <param name="dto">اطلاعات برند برای به‌روزرسانی</param>
        Task UpdateAsync(UpdateBrandDto dto);

        /// <summary>
        /// حذف برند
        /// </summary>
        /// <param name="id">شناسه برند</param>
        Task DeleteAsync(long id);
    }
}