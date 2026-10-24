using Application.Interfaces.Repositories.Identity;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Identity
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری کاربر (User) با استفاده از Entity Framework Core
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly MyShopDbContext _context;

        /// <summary>
        /// سازنده ریپازیتوری کاربر
        /// </summary>
        /// <param name="context">کانتکست دیتابیس</param>
        public UserRepository(MyShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت کاربر بر اساس شناسه شخص
        /// </summary>
        public async Task<User?> GetByPersonIdAsync(long personId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PersonId == personId);
        }

        /// <summary>
        /// افزودن کاربر جدید
        /// </summary>
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        /// <summary>
        /// به‌روزرسانی کاربر
        /// </summary>
        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}