using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Repositories.Identity;
using Application.Interfaces.Services;
using Infra.Persistence;
using Infra.Persistence.Repositories.Catalog;
using Infra.Persistence.Repositories.Identity;
using Infra.Persistence.Seed;
using Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Extensions;

/// <summary>
/// کلاس توسعه‌دهنده (Extension) برای ثبت وابستگی‌های لایه زیرساخت (Infrastructure)
/// این کلاس مسئول ثبت DbContext، ریپازیتوری‌ها، سرویس‌های زیرساختی و داده‌های اولیه (Seeder) در کانتینر تزریق وابستگی است.
/// </summary>
public static class InfrastructureDependencyInjection
{
    /// <summary>
    /// ثبت تمام سرویس‌ها و وابستگی‌های مربوط به لایه زیرساخت
    /// </summary>
    /// <param name="services">مجموعه سرویس‌های تزریق وابستگی (IServiceCollection)</param>
    /// <param name="configuration">پیکربندی برنامه برای خواندن رشته اتصال (Connection String) و سایر تنظیمات</param>
    /// <returns>مجموعه سرویس‌های به‌روزرسانی شده برای زنجیره‌سازی (Method Chaining)</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ۱. ثبت DbContext با استفاده از SQL Server
        services.AddDbContext<MyShopDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.MigrationsAssembly("Infra")
            ));

        // ۲. ثبت ریپازیتوری‌های ماژول کاتالوگ
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IFeatureValueRepository, FeatureValueRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // ثبت ریپازیتوری‌های ماژول هویت (Identity)
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // ۳. ثبت سرویس‌های زیرساختی (پیاده‌سازی‌های واقعی اینترفیس‌ها)
        services.AddScoped<IFileStorageService, FileStorageService>();
        // ثبت سرویس پیام (فعلاً نسخه موقت برای تست)
        services.AddScoped<IMessageService, FakeMessageService>();

        // ۴. ثبت سرویس مقداردهی اولیه داده‌ها (Seeder)
        services.AddScoped<ICatalogSeeder, CatalogDbSeeder>();

        return services;
    }
}