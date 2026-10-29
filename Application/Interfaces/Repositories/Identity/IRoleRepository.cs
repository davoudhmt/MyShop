using Domain.Entities.Identity;

namespace Application.Interfaces.Repositories.Identity
{
    /// <summary>
    /// واسط ریپازیتوری نقش (Role)
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// دریافت نقش بر اساس شناسه
        /// </summary>
        /// <param name="id">شناسه نقش</param>
        /// <returns>نقش یافت شده یا null</returns>
        Task<Role?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت نقش بر اساس نام (مثلاً "Admin")
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
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        Task SaveChangesAsync();
    }
}