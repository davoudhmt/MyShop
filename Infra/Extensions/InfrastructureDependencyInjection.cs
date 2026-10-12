using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services;
using Infra.Persistence;
using Infra.Persistence.Repositories.Catalog;
using Infra.Persistence.Seed;
using Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Extensions;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MyShopDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IFeatureValueRepository, FeatureValueRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IFileStorageService, FileStorageService>();

        services.AddScoped<ICatalogSeeder, CatalogDbSeeder>();

        return services;
    }
}