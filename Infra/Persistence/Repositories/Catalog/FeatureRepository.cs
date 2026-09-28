using Application.DTOs.Catalog.Feature;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Catalog
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری ویژگی
    /// </summary>
    public class FeatureRepository : IFeatureRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده ریپازیتوری ویژگی
        /// </summary>
        /// <param name="db">کانتکست دیتابیس</param>
        public FeatureRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت همه ویژگی‌ها
        /// </summary>
        public async Task<List<Feature>> GetAllAsync()
        {
            return await _db.Features
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت ویژگی با شناسه
        /// </summary>
        public async Task<Feature?> GetByIdAsync(long id)
        {
            return await _db.Features
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// دریافت ویژگی با نام
        /// </summary>
        public async Task<Feature?> GetByNameAsync(string name)
        {
            return await _db.Features
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Name == name);
        }

        /// <summary>
        /// دریافت ویژگی‌های یک نوع محصول خاص
        /// </summary>
        public async Task<List<Feature>> GetByProductTypeIdAsync(long productTypeId)
        {
            return await _db.Features
                .AsNoTracking()
                .Where(f => f.ProductTypes.Any(pt => pt.Id == productTypeId))
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت ویژگی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<Feature>> GetPagedAsync(FeatureFilterDto filter)
        {
            var query = _db.Features.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(f => f.Name.Contains(searchTerm));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(f => f.Status == status);
                }
            }

            // ===== فیلتر بر اساس نوع محصول =====
            if (filter.ProductTypeId.HasValue)
            {
                query = query.Where(f => f.ProductTypes.Any(pt => pt.Id == filter.ProductTypeId.Value));
            }

            // ===== مرتب‌سازی =====
            query = query.OrderBy(f => f.Name);

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Feature>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// افزودن ویژگی جدید
        /// </summary>
        public async Task AddAsync(Feature feature)
        {
            await _db.Features.AddAsync(feature);
        }

        /// <summary>
        /// به‌روزرسانی ویژگی
        /// </summary>
        public void Update(Feature feature)
        {
            _db.Features.Update(feature);
        }

        /// <summary>
        /// بررسی وجود ویژگی با نام
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Features.AnyAsync(f => f.Name == name);
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