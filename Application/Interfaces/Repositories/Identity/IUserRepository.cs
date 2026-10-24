using Domain.Entities.Identity;

namespace Application.Interfaces.Repositories.Identity
{
    /// <summary>
    /// واسط ریپازیتوری کاربر (User)
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// دریافت کاربر بر اساس شناسه شخص
        /// </summary>
        /// <param name="personId">شناسه شخص</param>
        /// <returns>کاربر یافت شده یا null</returns>
        Task<User?> GetByPersonIdAsync(long personId);

        /// <summary>
        /// افزودن کاربر جدید
        /// </summary>
        Task AddAsync(User user);

        /// <summary>
        /// به‌روزرسانی کاربر
        /// </summary>
        void Update(User user);

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        Task SaveChangesAsync();
    }
}