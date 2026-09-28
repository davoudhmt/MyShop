using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Catalog
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری تصویر محصول
    /// </summary>
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly MyShopDbContext _db;

        /// <summary>
        /// سازنده ریپازیتوری تصویر محصول
        /// </summary>
        public ProductImageRepository(MyShopDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// دریافت تصویر با شناسه
        /// </summary>
        public async Task<ProductImage?> GetByIdAsync(long id)
        {
            return await _db.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == id);
        }

        /// <summary>
        /// دریافت همه تصاویر یک محصول (اول تصویر اصلی، سپس بقیه)
        /// </summary>
        public async Task<List<ProductImage>> GetByProductIdAsync(long productId)
        {
            return await _db.ProductImages
                .AsNoTracking()
                .Where(pi => pi.ProductId == productId)
                .OrderByDescending(pi => pi.IsMain)
                .ThenBy(pi => pi.Id)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت تصویر اصلی (کاور) یک محصول
        /// </summary>
        public async Task<ProductImage?> GetMainImageByProductIdAsync(long productId)
        {
            return await _db.ProductImages
                .AsNoTracking()
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.IsMain);
        }

        /// <summary>
        /// دریافت تصاویر یک واریانت خاص (اول تصویر اصلی)
        /// </summary>
        public async Task<List<ProductImage>> GetByVariantIdAsync(long variantId)
        {
            return await _db.ProductImages
                .AsNoTracking()
                .Where(pi => pi.VariantId == variantId)
                .OrderByDescending(pi => pi.IsMain)
                .ThenBy(pi => pi.Id)
                .ToListAsync();
        }

        /// <summary>
        /// افزودن تصویر جدید
        /// </summary>
        public async Task AddAsync(ProductImage image)
        {
            await _db.ProductImages.AddAsync(image);
        }

        /// <summary>
        /// به‌روزرسانی تصویر
        /// </summary>
        public void Update(ProductImage image)
        {
            _db.ProductImages.Update(image);
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