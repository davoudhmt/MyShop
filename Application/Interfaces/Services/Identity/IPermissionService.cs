using Application.DTOs.Common;
using Application.DTOs.Identity.Permission;
using Domain.Enums;

namespace Application.Interfaces.Services.Identity
{
    /// <summary>
    /// واسط سرویس مدیریت مجوزها (Permissions)
    /// مسئولیت عملیات کسب‌وکاری مرتبط با مجوزهای سیستم
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// دریافت مجوزها با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        /// <returns>نتیجه صفحه‌بندی شده</returns>
        Task<PagedResult<PermissionDto>> GetPagedAsync(PermissionFilterDto filter);

        /// <summary>
        /// دریافت مجوز با شناسه
        /// </summary>
        /// <param name="id">شناسه مجوز</param>
        /// <returns>مجوز یا null</returns>
        Task<PermissionDto?> GetByIdAsync(long id);

        /// <summary>
        /// ساخت مجوز جدید
        /// </summary>
        /// <param name="dto">اطلاعات مجوز جدید</param>
        /// <returns>مجوز ساخته‌شده</returns>
        Task<PermissionDto> CreateAsync(CreatePermissionDto dto);

        /// <summary>
        /// به‌روزرسانی مجوز
        /// </summary>
        /// <param name="dto">اطلاعات مجوز برای به‌روزرسانی</param>
        Task UpdateAsync(UpdatePermissionDto dto);

        /// <summary>
        /// حذف مجوز
        /// </summary>
        /// <param name="id">شناسه مجوز</param>
        Task DeleteAsync(long id);

        /// <summary>
        /// دریافت مجوزها به صورت گروه‌بندی شده بر اساس ماژول
        /// این متد برای نمایش در پنل ادمین استفاده می‌شود
        /// </summary>
        /// <returns>دیکشنری که کلید آن ماژول و مقدار آن لیست مجوزهای آن ماژول است</returns>
        Task<IReadOnlyDictionary<Module, IReadOnlyList<PermissionDto>>> GetGroupedByModuleAsync();
    }
}