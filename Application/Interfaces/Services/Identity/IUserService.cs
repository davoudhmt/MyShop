using Application.DTOs.Common;
using Application.DTOs.Identity.User;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Identity
{
    /// <summary>
    /// واسط سرویس مدیریت کاربران (Users)
    /// مسئولیت عملیات کسب‌وکاری مرتبط با کاربران سیستم در پنل مدیریت
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// دریافت کاربران با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        /// <returns>نتیجه صفحه‌بندی شده</returns>
        Task<PagedResult<UserDto>> GetPagedAsync(UserFilterDto filter);

        /// <summary>
        /// دریافت کاربر با شناسه
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <returns>اطلاعات کاربر یا null</returns>
        Task<UserDto?> GetByIdAsync(long id);

        /// <summary>
        /// حذف منطقی کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        Task DeleteAsync(long id);

        /// <summary>
        /// فعال‌سازی کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        Task ActivateAsync(long id);

        /// <summary>
        /// غیرفعال‌سازی کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        Task DeactivateAsync(long id);
    }
}