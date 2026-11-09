using Application.DTOs.Common;
using Application.DTOs.Identity.Person; // <-- این نکسپیس را بعداً می‌سازیم
using Domain.Entities.Identity;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Identity
{
    /// <summary>
    /// واسط ریپازیتوری شخص (Person)
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// دریافت اشخاص با صفحه‌بندی و فیلتر (مخصوص پنل ادمین)
        /// </summary>
        Task<PagedResult<Person>> GetPagedAsync(PersonFilterDto filter);

        /// <summary>
        /// دریافت شخص بر اساس شماره موبایل
        /// </summary>
        Task<Person?> GetByMobileAsync(string mobile);

        /// <summary>
        /// دریافت شخص بر اساس شناسه
        /// </summary>
        Task<Person?> GetByIdAsync(long id);

        /// <summary>
        /// افزودن شخص جدید
        /// </summary>
        Task AddAsync(Person person);

        /// <summary>
        /// به‌روزرسانی شخص
        /// </summary>
        void Update(Person person);

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        Task SaveChangesAsync();
    }
}