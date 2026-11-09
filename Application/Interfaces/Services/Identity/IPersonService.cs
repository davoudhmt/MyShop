using Application.DTOs.Common;
using Application.DTOs.Identity.Person;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Identity
{
    /// <summary>
    /// واسط سرویس مدیریت اشخاص (Persons) در پنل مدیریت
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// دریافت اشخاص با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        Task<PagedResult<PersonDto>> GetPagedAsync(PersonFilterDto filter);

        /// <summary>
        /// دریافت شخص با شناسه
        /// </summary>
        Task<PersonDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت شخص بر اساس شماره موبایل
        /// </summary>
        Task<PersonDto?> GetByMobileAsync(string mobile);

        /// <summary>
        /// ساخت شخص جدید
        /// </summary>
        Task<PersonDto> CreateAsync(CreatePersonDto dto);

        /// <summary>
        /// به‌روزرسانی اطلاعات شخص
        /// </summary>
        Task UpdateAsync(UpdatePersonDto dto);

        /// <summary>
        /// حذف منطقی شخص
        /// </summary>
        Task DeleteAsync(long id);

        /// <summary>
        /// فعال‌سازی شخص
        /// </summary>
        Task ActivateAsync(long id);

        /// <summary>
        /// غیرفعال‌سازی شخص
        /// </summary>
        Task DeactivateAsync(long id);
    }
}