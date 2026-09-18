using Application.DTOs.Catalog.Category;
using Application.DTOs.Common;
using Domain.Entities.Catalog;

namespace Application.Interfaces.Repositories.Catalog
{
    /// <summary>
    /// واسط ریپازیتوری دسته‌بندی
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// دریافت همه دسته‌بندی‌ها
        /// </summary>
        Task<List<Category>> GetAllAsync();

        /// <summary>
        /// دریافت دسته‌بندی با شناسه
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        Task<Category?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت دسته‌بندی با نام
        /// </summary>
        /// <param name="name">نام دسته‌بندی</param>
        Task<Category?> GetByNameAsync(string name);

        /// <summary>
        /// دریافت دسته‌بندی با کد
        /// </summary>
        /// <param name="code">کد دسته‌بندی</param>
        Task<Category?> GetByCodeAsync(string code);

        /// <summary>
        /// دریافت زیردسته‌های یک دسته
        /// </summary>
        /// <param name="parentCategoryId">شناسه دسته والد</param>
        Task<List<Category>> GetChildrenAsync(long parentCategoryId);

        /// <summary>
        /// دریافت دسته‌بندی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<Category>> GetPagedAsync(CategoryFilterDto filter);

        /// <summary>
        /// افزودن دسته‌بندی جدید
        /// </summary>
        /// <param name="category">دسته‌بندی جدید</param>
        Task AddAsync(Category category);

        /// <summary>
        /// به‌روزرسانی دسته‌بندی
        /// </summary>
        /// <param name="category">دسته‌بندی برای به‌روزرسانی</param>
        void Update(Category category);

        /// <summary>
        /// بررسی وجود دسته‌بندی با نام
        /// </summary>
        /// <param name="name">نام دسته‌بندی</param>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// بررسی وجود دسته‌بندی با کد
        /// </summary>
        /// <param name="code">کد دسته‌بندی</param>
        Task<bool> ExistsByCodeAsync(string code);

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        Task SaveChangesAsync();
    }
}