using System.Threading.Tasks;

namespace Infra.Persistence.Seed
{
    /// <summary>
    /// واسط کلاس داده‌های اولیه ماژول کاتالوگ
    /// </summary>
    public interface ICatalogSeeder
    {
        /// <summary>
        /// متد اضافه کردن داده‌های اولیه به دیتابیس
        /// </summary>
        /// <returns>وظیفه ناهمزمان</returns>
        Task SeedAsync();
    }
}