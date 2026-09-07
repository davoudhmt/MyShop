using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infra.Persistence
{
    /// <summary>
    /// Factory برای ساخت DbContext در زمان طراحی (مایگریشن)
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyShopDbContext>
    {
        /// <summary>
        /// ساخت DbContext با کانفیگ زمان طراحی
        /// </summary>
        /// <param name="args">آرگومان‌های خط فرمان</param>
        /// <returns>DbContext ساخته‌شده</returns>
        public MyShopDbContext CreateDbContext(string[] args)
        {
            // current directory خودش پوشه API است (به خاطر --startup-project API)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MyShopDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new MyShopDbContext(optionsBuilder.Options);
        }
    }
}