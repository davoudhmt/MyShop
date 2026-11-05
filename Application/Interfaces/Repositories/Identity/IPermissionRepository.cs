using Application.DTOs.Common;
using Application.DTOs.Identity.Permission;
using Domain.Entities.Identity;
using Domain.Enums;

namespace Application.Interfaces.Repositories.Identity
{
    /// <summary>
    /// واسط ریپازیتوری مجوز (Permission)
    /// مسئولیت عملیات CRUD و کوئری‌های مرتبط با مجوزهای سیستم
    /// </summary>
    public interface IPermissionRepository
    {
        /// <summary>
        /// دریافت مجوزها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر و صفحه‌بندی</param>
        /// <returns>نتیجه صفحه‌بندی شده</returns>
        Task<PagedResult<Permission>> GetPagedAsync(PermissionFilterDto filter);

        /// <summary>
        /// دریافت مجوز بر اساس شناسه
        /// </summary>
        /// <param name="id">شناسه مجوز</param>
        /// <returns>مجوز یافت شده یا null</returns>
        Task<Permission?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت مجوز بر اساس نام (برای بررسی یونیک بودن)
        /// </summary>
        /// <param name="name">نام مجوز (مثلاً "orders.view")</param>
        /// <returns>مجوز یافت شده یا null</returns>
        Task<Permission?> GetByNameAsync(string name);

        /// <summary>
        /// دریافت تمام مجوزهای سیستم
        /// </summary>
        /// <returns>لیست تمام مجوزها</returns>
        Task<IReadOnlyList<Permission>> GetAllAsync();

        /// <summary>
        /// دریافت مجوزها به صورت گروه‌بندی شده بر اساس ماژول
        /// این متد برای نمایش در پنل ادمین و مدیریت راحت‌تر مجوزها استفاده می‌شود
        /// </summary>
        /// <returns>دیکشنری که کلید آن نام ماژول و مقدار آن لیست مجوزهای آن ماژول است</returns>
        Task<IReadOnlyDictionary<Module, IReadOnlyList<Permission>>> GetGroupedByModuleAsync();

        /// <summary>
        /// افزودن مجوز جدید
        /// </summary>
        /// <param name="permission">موجودیت مجوز</param>
        Task AddAsync(Permission permission);

        /// <summary>
        /// به‌روزرسانی مجوز موجود
        /// </summary>
        /// <param name="permission">موجودیت مجوز با تغییرات</param>
        void Update(Permission permission);

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        Task SaveChangesAsync();
    }
}