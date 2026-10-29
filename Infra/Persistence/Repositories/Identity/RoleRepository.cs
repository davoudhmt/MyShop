using Application.Interfaces.Repositories.Identity;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Identity
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری نقش (Role) با استفاده از Entity Framework Core
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly MyShopDbContext _context;

        /// <summary>
        /// سازنده ریپازیتوری نقش
        /// </summary>
        /// <param name="context">کانتکست دیتابیس</param>
        public RoleRepository(MyShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت نقش بر اساس شناسه
        /// </summary>
        public async Task<Role?> GetByIdAsync(long id)
        {
            return await _context.Roles.FindAsync(id);
        }

        /// <summary>
        /// دریافت نقش بر اساس نام (مثلاً "Admin")
        /// </summary>
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        /// <summary>
        /// افزودن نقش جدید
        /// </summary>
        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
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