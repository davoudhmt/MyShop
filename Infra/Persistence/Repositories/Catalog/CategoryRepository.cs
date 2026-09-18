using Application.DTOs.Catalog.Category;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Catalog
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری دسته‌بندی
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده ریپازیتوری دسته‌بندی
        /// </summary>
        /// <param name="db">کانتکست دیتابیس</param>
        public CategoryRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت همه دسته‌بندی‌ها
        /// </summary>
        public async Task<List<Category>> GetAllAsync()
        {
            return await _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت دسته‌بندی با شناسه (همراه فرزندان و پدر برای قوانین دامنه)
        /// </summary>
        public async Task<Category?> GetByIdAsync(long id)
        {
            return await _db.Categories
                .Include(c => c.Children)
                    .ThenInclude(c => c.Children)
                    .ThenInclude(c => c.Children)
                .Include(c => c.Parent)
                    .ThenInclude(c => c!.Parent)
                    .ThenInclude(c => c!.Parent)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// دریافت دسته‌بندی با نام
        /// </summary>
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _db.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        /// <summary>
        /// دریافت دسته‌بندی با کد
        /// </summary>
        public async Task<Category?> GetByCodeAsync(string code)
        {
            return await _db.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryCode.Value == code);
        }

        /// <summary>
        /// دریافت زیردسته‌های یک دسته
        /// </summary>
        public async Task<List<Category>> GetChildrenAsync(long parentCategoryId)
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت دسته‌بندی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<Category>> GetPagedAsync(CategoryFilterDto filter)
        {
            var query = _db.Categories.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(c => c.Name.Contains(searchTerm));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(c => c.Status == status);
                }
            }

            // ===== فیلتر درختی =====
            if (filter.OnlyRoots)
            {
                query = query.Where(c => c.ParentCategoryId == null);
            }
            else if (filter.ParentCategoryId.HasValue)
            {
                query = query.Where(c => c.ParentCategoryId == filter.ParentCategoryId);
            }

            // ===== مرتب‌سازی =====
            query = query.OrderBy(c => c.Name);

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Category>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// افزودن دسته‌بندی جدید
        /// </summary>
        public async Task AddAsync(Category category)
        {
            await _db.Categories.AddAsync(category);
        }

        /// <summary>
        /// به‌روزرسانی دسته‌بندی
        /// </summary>
        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }

        /// <summary>
        /// بررسی وجود دسته‌بندی با نام
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Categories.AnyAsync(c => c.Name == name);
        }

        /// <summary>
        /// بررسی وجود دسته‌بندی با کد
        /// </summary>
        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _db.Categories.AnyAsync(c => c.CategoryCode.Value == code);
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