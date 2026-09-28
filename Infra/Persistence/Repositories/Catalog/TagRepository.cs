using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Catalog
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری برچسب
    /// </summary>
    public class TagRepository : ITagRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده ریپازیتوری برچسب
        /// </summary>
        /// <param name="db">کانتکست دیتابیس</param>
        public TagRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت همه برچسب‌ها
        /// </summary>
        public async Task<List<Tag>> GetAllAsync()
        {
            return await _db.Tags
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت برچسب با شناسه
        /// </summary>
        public async Task<Tag?> GetByIdAsync(long id)
        {
            return await _db.Tags
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// دریافت برچسب‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<Tag>> GetPagedAsync(TagFilterDto filter)
        {
            var query = _db.Tags.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(t => t.Name.Contains(searchTerm));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(t => t.Status == status);
                }
            }

            // ===== مرتب‌سازی =====
            query = query.OrderBy(t => t.Name);

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Tag>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// افزودن برچسب جدید
        /// </summary>
        public async Task AddAsync(Tag tag)
        {
            await _db.Tags.AddAsync(tag);
        }

        /// <summary>
        /// به‌روزرسانی برچسب
        /// </summary>
        public void Update(Tag tag)
        {
            _db.Tags.Update(tag);
        }

        /// <summary>
        /// بررسی وجود برچسب با این نام
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Tags
                .AnyAsync(t => t.Name == name);
        }

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}