using Application.DTOs.Common;
using Application.DTOs.Identity.Role;
using Application.Interfaces.Repositories.Identity;
using Domain.Entities.Identity;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        /// دریافت نقش‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<Role>> GetPagedAsync(RoleFilterDto filter)
        {
            var query = _context.Roles.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو در نام یا توضیحات =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(r => r.Name.Contains(searchTerm) || (r.Description != null && r.Description.Contains(searchTerm)));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(r => r.Status == status);
                }
            }

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== اعتبارسنجی و تنظیم صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            // ===== دریافت آیتم‌های صفحه فعلی =====
            var items = await query
                .OrderBy(r => r.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Role>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// دریافت نقش بر اساس شناسه (ساده)
        /// </summary>
        public async Task<Role?> GetByIdAsync(long id)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// دریافت نقش بر اساس شناسه به همراه مجوزهای تخصیص‌یافته (برای جزئیات)
        /// </summary>
        public async Task<Role?> GetDetailsAsync(long id)
        {
            return await _context.Roles
                .AsNoTracking()
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// دریافت نقش بر اساس نام (برای بررسی یونیک بودن)
        /// </summary>
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles
                .AsNoTracking()
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
        /// به‌روزرسانی نقش موجود
        /// </summary>
        public void Update(Role role)
        {
            _context.Roles.Update(role);
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