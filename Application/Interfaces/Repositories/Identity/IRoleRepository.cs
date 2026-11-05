using Application.DTOs.Common;
using Application.DTOs.Identity.Role;
using Domain.Entities.Identity;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Identity
{
    /// <summary>
    /// واسط ریپازیتوری نقش (Role)
    /// مسئولیت عملیات CRUD و کوئری‌های مرتبط با نقش‌های سیستم
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// دریافت نقش‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر و صفحه‌بندی</param>
        /// <returns>نتیجه صفحه‌بندی شده</returns>
        Task<PagedResult<Role>> GetPagedAsync(RoleFilterDto filter);

        /// <summary>
        /// دریافت نقش بر اساس شناسه (ساده)
        /// </summary>
        /// <param name="id">شناسه نقش</param>
        /// <returns>نقش یافت شده یا null</returns>
        Task<Role?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت نقش بر اساس شناسه به همراه مجوزهای تخصیص‌یافته (برای نمایش جزئیات)
        /// </summary>
        /// <param name="id">شناسه نقش</param>
        /// <returns>نقش به همراه لیست مجوزها یا null</returns>
        Task<Role?> GetDetailsAsync(long id);

        /// <summary>
        /// دریافت نقش بر اساس نام (برای بررسی یونیک بودن)
        /// </summary>
        /// <param name="name">نام نقش</param>
        /// <returns>نقش یافت شده یا null</returns>
        Task<Role?> GetByNameAsync(string name);

        /// <summary>
        /// افزودن نقش جدید
        /// </summary>
        /// <param name="role">موجودیت نقش</param>
        Task AddAsync(Role role);

        /// <summary>
        /// به‌روزرسانی نقش موجود
        /// </summary>
        /// <param name="role">موجودیت نقش با تغییرات</param>
        void Update(Role role);

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        Task SaveChangesAsync();
    }
}