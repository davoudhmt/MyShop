using Domain.Entities.Identity;

namespace Application.Interfaces.Repositories.Identity
{
    /// <summary>
    /// واسط ریپازیتوری شخص (Person)
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// دریافت شخص بر اساس شماره موبایل
        /// </summary>
        /// <param name="mobile">شماره موبایل</param>
        /// <returns>شخص یافت شده یا null</returns>
        Task<Person?> GetByMobileAsync(string mobile);

        /// <summary>
        /// دریافت شخص بر اساس شناسه
        /// </summary>
        /// <param name="id">شناسه شخص</param>
        /// <returns>شخص یافت شده یا null</returns>
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