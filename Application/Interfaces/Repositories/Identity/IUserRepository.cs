using Domain.Entities.Identity;

namespace Application.Interfaces.Repositories.Identity
{
    /// <summary>
    /// واسط ریپازیتوری کاربر (User)
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// دریافت کاربر بر اساس شناسه
        /// </summary>
        Task<User?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت کاربر بر اساس شناسه شخص
        /// </summary>
        /// <param name="personId">شناسه شخص</param>
        /// <returns>کاربر یافت شده یا null</returns>
        Task<User?> GetByPersonIdAsync(long personId);

        /// <summary>
        /// دریافت کاربر بر اساس توکن نوسازی (Refresh Token)
        /// </summary>
        /// <param name="refreshToken">مقدار توکن نوسازی</param>
        /// <returns>موجودیت کاربر یا null در صورت عدم یافتن</returns>
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

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