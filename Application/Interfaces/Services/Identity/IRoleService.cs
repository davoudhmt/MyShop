using Application.DTOs.Common;
using Application.DTOs.Identity.Role;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Identity
{
    /// <summary>
    /// واسط سرویس مدیریت نقش‌ها (Roles)
    /// مسئولیت عملیات کسب‌وکاری مرتبط با نقش‌های سیستم
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// دریافت نقش‌ها با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        /// <returns>نتیجه صفحه‌بندی شده</returns>
        Task<PagedResult<RoleDto>> GetPagedAsync(RoleFilterDto filter);

        /// <summary>
        /// دریافت نقش با شناسه (خلاصه)
        /// </summary>
        /// <param name="id">شناسه نقش</param>
        /// <returns>نقش یا null</returns>
        Task<RoleDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت جزئیات کامل نقش به همراه لیست مجوزهای تخصیص‌یافته
        /// </summary>
        /// <param name="id">شناسه نقش</param>
        /// <returns>جزئیات نقش یا null</returns>
        Task<RoleDetailsDto?> GetDetailsAsync(long id);

        /// <summary>
        /// ساخت نقش جدید
        /// </summary>
        /// <param name="dto">اطلاعات نقش جدید</param>
        /// <returns>نقش ساخته‌شده</returns>
        Task<RoleDto> CreateAsync(CreateRoleDto dto);

        /// <summary>
        /// به‌روزرسانی نقش موجود
        /// </summary>
        /// <param name="dto">اطلاعات نقش برای به‌روزرسانی</param>
        Task UpdateAsync(UpdateRoleDto dto);

        /// <summary>
        /// حذف منطقی نقش
        /// </summary>
        /// <param name="id">شناسه نقش</param>
        Task DeleteAsync(long id);
    }
}