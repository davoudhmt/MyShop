using Application.DTOs.Catalog.Product;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Catalog
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری محصول
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده ریپازیتوری محصول
        /// </summary>
        /// <param name="db">کانتکست دیتابیس</param>
        public ProductRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت محصول با شناسه (همراه با داده‌های مرتبط)
        /// </summary>
        public async Task<Product?> GetByIdAsync(long id)
        {
            return await _db.Products
                .AsNoTracking()
                .Include(p => p.Tags)
                .Include(p => p.ProductImages)
                .Include(p => p.FeatureValues)
                    .ThenInclude(fv => fv.Feature)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// دریافت محصولات با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<Product>> GetPagedAsync(ProductFilterDto filter)
        {
            var query = _db.Products.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو در نام یا کد محصول =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(p => p.Name.Contains(searchTerm) || p.ProductCode.Value.Contains(searchTerm));
            }

            // ===== فیلتر دسته‌بندی =====
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            // ===== فیلتر برند =====
            if (filter.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == filter.BrandId.Value);
            }

            // ===== فیلتر نوع محصول =====
            if (filter.ProductTypeId.HasValue)
            {
                query = query.Where(p => p.ProductTypeId == filter.ProductTypeId.Value);
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(p => p.Status == status);
                }
            }

            // ===== مرتب‌سازی بر اساس تاریخ ایجاد (جدیدترین اول) =====
            query = query.OrderByDescending(p => p.CreateDate);

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Include(p => p.Tags)
                .Include(p => p.ProductImages)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// دریافت محصول با اسلاگ (آدرس URL)
        /// </summary>
        public async Task<Product?> GetBySlugAsync(string slug)
        {
            return await _db.Products
                .AsNoTracking()
                .Include(p => p.Tags)
                .Include(p => p.ProductImages)
                .Include(p => p.FeatureValues)
                    .ThenInclude(fv => fv.Feature)
                .FirstOrDefaultAsync(p => p.Slug.Value == slug);
        }

        /// <summary>
        /// بررسی وجود محصول با کد مشخص
        /// </summary>
        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _db.Products
                .AnyAsync(p => p.ProductCode.Value == productCode);
        }

        /// <summary>
        /// بررسی وجود محصول با اسلاگ مشخص
        /// </summary>
        public async Task<bool> ExistsBySlugAsync(string slug)
        {
            return await _db.Products
                .AnyAsync(p => p.Slug.Value == slug);
        }

        /// <summary>
        /// افزودن محصول جدید
        /// </summary>
        public async Task AddAsync(Product product)
        {
            await _db.Products.AddAsync(product);
        }

        /// <summary>
        /// به‌روزرسانی محصول
        /// </summary>
        public void Update(Product product)
        {
            _db.Products.Update(product);
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