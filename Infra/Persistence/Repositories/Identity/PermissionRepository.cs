using Application.DTOs.Common;
using Application.DTOs.Identity.Permission;
using Application.Interfaces.Repositories.Identity;
using Domain.Entities.Identity;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories.Identity
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری مجوز (Permission) با استفاده از Entity Framework Core
    /// </summary>
    public class PermissionRepository : IPermissionRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده ریپازیتوری مجوز
        /// </summary>
        /// <param name="context">کانتکست دیتابیس</param>
        public PermissionRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت مجوزها با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<Permission>> GetPagedAsync(PermissionFilterDto filter)
        {
            var query = _db.Permissions.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو در نام یا نام نمایشی =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(p => p.Name.Contains(searchTerm) || p.DisplayName.Contains(searchTerm));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(p => p.Status == status);
                }
            }

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== اعتبارسنجی و تنظیم صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            // ===== دریافت آیتم‌های صفحه فعلی =====
            var items = await query
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Permission>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// دریافت مجوز بر اساس شناسه
        /// </summary>
        public async Task<Permission?> GetByIdAsync(long id)
        {
            return await _db.Permissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// دریافت مجوز بر اساس نام (برای بررسی یونیک بودن)
        /// </summary>
        public async Task<Permission?> GetByNameAsync(string name)
        {
            return await _db.Permissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        /// <summary>
        /// دریافت تمام مجوزهای سیستم
        /// </summary>
        public async Task<IReadOnlyList<Permission>> GetAllAsync()
        {
            return await _db.Permissions
                .AsNoTracking()
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت مجوزها به صورت گروه‌بندی شده بر اساس ماژول
        /// </summary>
        public async Task<IReadOnlyDictionary<Module, IReadOnlyList<Permission>>> GetGroupedByModuleAsync()
        {
            // دریافت داده‌ها با AsNoTracking و مرتب‌سازی اولیه برای تضمین ترتیب قابل پیش‌بینی
            var permissions = await _db.Permissions
                .AsNoTracking()
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();

            // گروه‌بندی در حافظه (ایمن‌ترین روش برای جلوگیری از خطای ترجمه پیچیده در EF Core)
            return permissions
                .GroupBy(p => p.Module)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<Permission>)g.ToList()
                );
        }

        /// <summary>
        /// افزودن مجوز جدید
        /// </summary>
        public async Task AddAsync(Permission permission)
        {
            await _db.Permissions.AddAsync(permission);
        }

        /// <summary>
        /// به‌روزرسانی مجوز موجود
        /// </summary>
        public void Update(Permission permission)
        {
            _db.Permissions.Update(permission);
        }

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}