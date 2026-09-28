using Domain.Entities.Catalog;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence
{
    /// <summary>
    /// DbContext اصلی فروشگاه MyShop
    /// </summary>
    public class MyShopDbContext : DbContext
    {
        /// <summary>
        /// سازنده DbContext با گزینه‌های اتصال
        /// </summary>
        /// <param name="options">گزینه‌های پیکربندی DbContext</param>
        public MyShopDbContext(DbContextOptions<MyShopDbContext> options) : base(options)
        {
        }

        // ===== موجودیت‌های ماژول Catalog =====

        /// <summary>
        /// مجموعه برندها
        /// </summary>
        public DbSet<Brand> Brands => Set<Brand>();

        /// <summary>
        /// مجموعه دسته‌بندی‌ها
        /// </summary>
        public DbSet<Category> Categories => Set<Category>();

        /// <summary>
        /// مجموعه انواع محصول
        /// </summary>
        public DbSet<ProductType> ProductTypes => Set<ProductType>();

        /// <summary>
        /// مجموعه ویژگی‌ها
        /// </summary>
        public DbSet<Feature> Features => Set<Feature>();

        /// <summary>
        /// مجموعه مقادیر ویژگی‌ها
        /// </summary>
        public DbSet<FeatureValue> FeatureValues => Set<FeatureValue>();

        /// <summary>
        /// مجموعه واریانت‌ها
        /// </summary>
        public DbSet<Variant> Variants => Set<Variant>();

        /// <summary>
        /// مجموعه تصاویر محصولات
        /// </summary>
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        /// <summary>
        /// مجموعه محصولات
        /// </summary>
        public DbSet<Product> Products => Set<Product>();

        /// <summary>
        /// مجموعه برچسب‌ها
        /// </summary>
        public DbSet<Tag> Tags => Set<Tag>();

        /// <summary>
        /// متد پیکربندی مدل دیتابیس
        /// </summary>
        /// <param name="modelBuilder">سازنده مدل</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // اعمال خودکار همه کانفیگ‌های موجود در همین اسمبلی
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyShopDbContext).Assembly);

            // ===== فیلتر سراسری: رکوردهای حذف‌شده هیچ‌جا نمایش داده نشوند =====
            modelBuilder.Entity<Brand>().HasQueryFilter(b => b.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => c.Status != EntityStatus.Deleted);
            modelBuilder.Entity<ProductType>().HasQueryFilter(pt => pt.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Feature>().HasQueryFilter(f => f.Status != EntityStatus.Deleted);
            modelBuilder.Entity<FeatureValue>().HasQueryFilter(fv => fv.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Tag>().HasQueryFilter(t => t.Status != EntityStatus.Deleted);

            base.OnModelCreating(modelBuilder);
        }
    }
}