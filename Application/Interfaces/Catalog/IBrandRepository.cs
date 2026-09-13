using Application.DTOs.Catalog.Brand;
using Application.DTOs.Common;
using Domain.Entities.Catalog;

namespace Application.Interfaces.Repositories.Catalog
{
    /// <summary>
    /// واسط ریپازیتوری برند
    /// </summary>
    public interface IBrandRepository
    {
        /// <summary>
        /// دریافت همه برندها
        /// </summary>
        Task<List<Brand>> GetAllAsync();

        /// <summary>
        /// دریافت برند با شناسه
        /// </summary>
        /// <param name="id">شناسه برند</param>
        Task<Brand?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت برند با نام
        /// </summary>
        /// <param name="name">نام برند</param>
        Task<Brand?> GetByNameAsync(string name);

        /// <summary>
        /// افزودن برند جدید
        /// </summary>
        /// <param name="brand">برند جدید</param>
        Task AddAsync(Brand brand);

        /// <summary>
        /// به‌روزرسانی برند
        /// </summary>
        /// <param name="brand">برند برای به‌روزرسانی</param>
        void Update(Brand brand);

        /// <summary>
        /// حذف برند
        /// </summary>
        /// <param name="brand">برند برای حذف</param>
        void Remove(Brand brand);

        /// <summary>
        /// بررسی وجود برند با نام
        /// </summary>
        /// <param name="name">نام برند</param>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// دریافت برندها با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر و صفحه‌بندی</param>
        /// <returns>نتیجه صفحه‌بندی شده</returns>
        Task<PagedResult<Brand>> GetPagedAsync(BrandFilterDto filter);
    }
}