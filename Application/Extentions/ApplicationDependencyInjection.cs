using Application.Interfaces.Services.Catalog;
using Application.Services.Catalog;
using Application.Validators.Catalog.Brand;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // فقط سرویس‌های لایه Application
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductTypeService, ProductTypeService>();
        services.AddScoped<IFeatureService, FeatureService>();
        services.AddScoped<IFeatureValueService, FeatureValueService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<IProductService, ProductService>();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateBrandValidator>();
        services.AddFluentValidationAutoValidation();

        return services;
    }
}