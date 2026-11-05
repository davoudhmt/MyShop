using Application.DTOs.Common;
using Application.DTOs.Identity.User;
using Application.Interfaces.Repositories.Identity;
using Domain.Entities.Identity;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
        /// دریافت کاربران با صفحه‌بندی و فیلتر (مخصوص پنل ادمین)
        /// </summary>
        public async Task<PagedResult<User>> GetPagedAsync(UserFilterDto filter)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو در نام کاربری (موبایل) =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(u => u.Username.Contains(searchTerm));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (System.Enum.TryParse<Domain.Enums.EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(u => u.Status == status);
                }
            }

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== اعتبارسنجی و تنظیم صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            // ===== دریافت آیتم‌های صفحه فعلی =====
            var items = await query
                .OrderByDescending(u => u.CreateDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// دریافت کاربر بر اساس شناسه
        /// </summary>
        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// دریافت کاربر بر اساس شناسه شخص
        /// </summary>
        public async Task<User?> GetByPersonIdAsync(long personId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.PersonId == personId);
        }

        /// <summary>
        /// دریافت کاربر بر اساس توکن نوسازی (Refresh Token)
        /// </summary>
        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        /// <summary>
        /// دریافت کاربر بر اساس شماره موبایل، همراه با بارگذاری صریح نقش‌ها
        /// </summary>
        public async Task<User?> GetByMobileWithRolesAsync(string mobile)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == mobile);
        }

        /// <summary>
        /// دریافت کاربر بر اساس شناسه، همراه با بارگذاری صریح نقش‌ها
        /// </summary>
        public async Task<User?> GetByIdWithRolesAsync(long id)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// افزودن نقش به کاربر
        /// </summary>
        public async Task AddRoleToUserAsync(long userId, long roleId)
        {
            // بررسی تکراری نبودن
            var exists = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (!exists)
            {
                var userRole = new UserRole(createBy: userId, userId: userId, roleId: roleId);
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();
            }
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