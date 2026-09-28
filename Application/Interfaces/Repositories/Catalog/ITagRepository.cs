using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;
using Domain.Entities.Catalog;

namespace Application.Interfaces.Repositories.Catalog
{
    /// <summary>
    /// واسط ریپازیتوری برچسب
    /// </summary>
    public interface ITagRepository
    {
        /// <summary>
        /// دریافت همه برچسب‌ها
        /// </summary>
        Task<List<Tag>> GetAllAsync();

        /// <summary>
        /// دریافت برچسب با شناسه
        /// </summary>
        /// <param name="id">شناسه برچسب</param>
        Task<Tag?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت برچسب‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<Tag>> GetPagedAsync(TagFilterDto filter);

        /// <summary>
        /// افزودن برچسب جدید
        /// </summary>
        /// <param name="tag">برچسب جدید</param>
        Task AddAsync(Tag tag);

        /// <summary>
        /// به‌روزرسانی برچسب
        /// </summary>
        /// <param name="tag">برچسب برای به‌روزرسانی</param>
        void Update(Tag tag);

        /// <summary>
        /// بررسی وجود برچسب با این نام
        /// </summary>
        /// <param name="name">نام برچسب</param>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// دریافت لیست برچسب‌ها بر اساس شناسه‌های داده شده
        /// </summary>
        Task<List<Tag>> GetByIdsAsync(IEnumerable<long> ids);

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        Task SaveChangesAsync();
    }
}