using Domain.Entities.Catalog;
using Domain.Entities.Identity;
using Domain.Entities.Location;
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

        // ===== موجودیت‌های ماژول Identity =====

        /// <summary>
        /// مجموعه اشخاص
        /// </summary>
        public DbSet<Person> Persons { get; set; } = null!;

        /// <summary>
        /// مجموعه‌ی کاربران سیستم (احراز هویت)
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// مجموعه‌ی آدرس‌های اشخاص
        /// </summary>
        public DbSet<Address> Addresses { get; set; } = null!;

        /// <summary>
        /// مجموعه‌ی نقش‌های کاربری
        /// </summary>
        public DbSet<Role> Roles { get; set; } = null!;

        /// <summary>
        /// مجموعه‌ی مجوزهای سیستم
        /// </summary>
        public DbSet<Permission> Permissions { get; set; } = null!;

        /// <summary>
        /// مجموعه‌ی ارتباط نقش‌ها به کاربران (جدول واسط)
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; } = null!;

        /// <summary>
        /// مجموعه‌ی ارتباط مجوزها به نقش‌ها (جدول واسط)
        /// </summary>
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;

        // ===== موجودیت‌های ماژول Location =====

        /// <summary>
        /// مجموعه‌ی مناطق جغرافیایی (استان‌ها و شهرها)
        /// </summary>
        public DbSet<Region> Regions { get; set; } = null!;

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
            modelBuilder.Entity<ProductImage>().HasQueryFilter(pi => pi.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Variant>().HasQueryFilter(v => v.Status != EntityStatus.Deleted);

            modelBuilder.Entity<Person>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => u.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Permission>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Role>().HasQueryFilter(r => r.Status != EntityStatus.Deleted);
            modelBuilder.Entity<UserRole>().HasQueryFilter(ur => ur.Status != EntityStatus.Deleted);
            modelBuilder.Entity<RolePermission>().HasQueryFilter(rp => rp.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Address>().HasQueryFilter(a => a.Status != EntityStatus.Deleted);  

            modelBuilder.Entity<Region>().HasQueryFilter(r => r.Status != EntityStatus.Deleted);

            base.OnModelCreating(modelBuilder);
        }
    }
}