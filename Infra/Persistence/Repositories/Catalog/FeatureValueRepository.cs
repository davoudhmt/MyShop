using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Catalog
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری مقدار ویژگی
    /// </summary>
    public class FeatureValueRepository : IFeatureValueRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده ریپازیتوری مقدار ویژگی
        /// </summary>
        /// <param name="db">کانتکست دیتابیس</param>
        public FeatureValueRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت همه مقادیر ویژگی
        /// </summary>
        public async Task<List<FeatureValue>> GetAllAsync()
        {
            return await _db.FeatureValues
                .AsNoTracking()
                .OrderBy(fv => fv.Value)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت مقدار ویژگی با شناسه
        /// </summary>
        public async Task<FeatureValue?> GetByIdAsync(long id)
        {
            return await _db.FeatureValues
                .FirstOrDefaultAsync(fv => fv.Id == id);
        }

        /// <summary>
        /// دریافت مقادیر یک ویژگی خاص
        /// </summary>
        public async Task<List<FeatureValue>> GetByFeatureIdAsync(long featureId)
        {
            return await _db.FeatureValues
                .AsNoTracking()
                .Where(fv => fv.FeatureId == featureId)
                .OrderBy(fv => fv.Value)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت مقادیر ویژگی با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<FeatureValue>> GetPagedAsync(FeatureValueFilterDto filter)
        {
            var query = _db.FeatureValues.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(fv => fv.Value.Contains(searchTerm));
            }

            // ===== فیلتر بر اساس ویژگی =====
            if (filter.FeatureId.HasValue)
            {
                query = query.Where(fv => fv.FeatureId == filter.FeatureId.Value);
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(fv => fv.Status == status);
                }
            }

            // ===== مرتب‌سازی =====
            query = query.OrderBy(fv => fv.Value);

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<FeatureValue>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// افزودن مقدار ویژگی جدید
        /// </summary>
        public async Task AddAsync(FeatureValue featureValue)
        {
            await _db.FeatureValues.AddAsync(featureValue);
        }

        /// <summary>
        /// به‌روزرسانی مقدار ویژگی
        /// </summary>
        public void Update(FeatureValue featureValue)
        {
            _db.FeatureValues.Update(featureValue);
        }

        /// <summary>
        /// بررسی وجود مقدار برای یک ویژگی خاص
        /// </summary>
        public async Task<bool> ExistsAsync(long featureId, string value)
        {
            return await _db.FeatureValues
                .AnyAsync(fv => fv.FeatureId == featureId && fv.Value == value);
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