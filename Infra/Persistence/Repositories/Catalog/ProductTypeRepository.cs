using Application.DTOs.Catalog.ProductType;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Catalog
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری نوع محصول
    /// </summary>
    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده ریپازیتوری نوع محصول
        /// </summary>
        /// <param name="db">کانتکست دیتابیس</param>
        public ProductTypeRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت همه انواع محصول
        /// </summary>
        public async Task<List<ProductType>> GetAllAsync()
        {
            return await _db.ProductTypes
                .AsNoTracking()
                .OrderBy(pt => pt.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت نوع محصول با شناسه
        /// </summary>
        public async Task<ProductType?> GetByIdAsync(long id)
        {
            return await _db.ProductTypes
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        /// <summary>
        /// دریافت نوع محصول با نام
        /// </summary>
        public async Task<ProductType?> GetByNameAsync(string name)
        {
            return await _db.ProductTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(pt => pt.Name == name);
        }

        /// <summary>
        /// دریافت انواع محصول یک دسته‌بندی خاص
        /// </summary>
        public async Task<List<ProductType>> GetByCategoryIdAsync(long categoryId)
        {
            return await _db.ProductTypes
                .AsNoTracking()
                .Where(pt => pt.Categories.Any(c => c.Id == categoryId))
                .OrderBy(pt => pt.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت انواع محصول با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<ProductType>> GetPagedAsync(ProductTypeFilterDto filter)
        {
            var query = _db.ProductTypes.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(pt => pt.Name.Contains(searchTerm));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(pt => pt.Status == status);
                }
            }

            // ===== فیلتر بر اساس دسته‌بندی =====
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(pt => pt.Categories.Any(c => c.Id == filter.CategoryId.Value));
            }

            // ===== مرتب‌سازی =====
            query = query.OrderBy(pt => pt.Name);

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductType>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// افزودن نوع محصول جدید
        /// </summary>
        public async Task AddAsync(ProductType productType)
        {
            await _db.ProductTypes.AddAsync(productType);
        }

        /// <summary>
        /// به‌روزرسانی نوع محصول
        /// </summary>
        public void Update(ProductType productType)
        {
            _db.ProductTypes.Update(productType);
        }

        /// <summary>
        /// بررسی وجود نوع محصول با نام
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.ProductTypes.AnyAsync(pt => pt.Name == name);
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