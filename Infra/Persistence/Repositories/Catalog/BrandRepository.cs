using Application.DTOs.Catalog.Brand;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Catalog
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری برند
    /// </summary>
    public class BrandRepository : IBrandRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده کلاس
        /// </summary>
        /// <param name="db">DbContext فروشگاه</param>
        public BrandRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت همه برندها
        /// </summary>
        public async Task<List<Brand>> GetAllAsync()
        {
            return await _db.Brands
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// دریافت برند با شناسه
        /// </summary>
        public async Task<Brand?> GetByIdAsync(long id)
        {
            return await _db.Brands
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <summary>
        /// دریافت برند با نام
        /// </summary>
        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _db.Brands
                .FirstOrDefaultAsync(b => b.Name == name);
        }

        /// <summary>
        /// افزودن برند جدید
        /// </summary>
        public async Task AddAsync(Brand brand)
        {
            await _db.Brands.AddAsync(brand);
        }

        /// <summary>
        /// به‌روزرسانی برند
        /// </summary>
        public void Update(Brand brand)
        {
            _db.Brands.Update(brand);
        }

        /// <summary>
        /// حذف برند
        /// </summary>
        public void Remove(Brand brand)
        {
            _db.Brands.Remove(brand);
        }

        /// <summary>
        /// بررسی وجود برند با نام
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Brands.AnyAsync(b => b.Name == name);
        }

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// دریافت برندها با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        public async Task<PagedResult<Brand>> GetPagedAsync(BrandFilterDto filter)
        {
            // شروع کوئری
            var query = _db.Brands.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(b => b.Name.Contains(searchTerm));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(b => b.Status == status);
                }
            }

            // ===== مرتب‌سازی =====
            query = query.OrderBy(b => b.Name);

            // ===== شمارش کل =====
            var totalCount = await query.CountAsync();

            // ===== صفحه‌بندی =====
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ===== ساخت نتیجه =====
            return new PagedResult<Brand>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}