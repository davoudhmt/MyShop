using Application.Interfaces.Services.Catalog;
using Application.Interfaces.Services.Identity;
using Application.Services.Catalog;
using Application.Services.Identity;
using Application.Validators.Catalog.Brand;
using Application.Validators.Identity.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

/// <summary>
/// کلاس توسعه‌دهنده (Extension) برای ثبت وابستگی‌های لایه برنامه (Application)
/// این کلاس مسئول ثبت سرویس‌های کسب‌وکار و تنظیمات اعتبارسنجی (Validation) در کانتینر تزریق وابستگی است.
/// </summary>
public static class ApplicationDependencyInjection
{
    /// <summary>
    /// ثبت تمام سرویس‌ها و تنظیمات اعتبارسنجی مربوط به لایه Application
    /// </summary>
    /// <param name="services">مجموعه سرویس‌های تزریق وابستگی (IServiceCollection)</param>
    /// <returns>مجموعه سرویس‌های به‌روزرسانی شده برای زنجیره‌سازی (Method Chaining)</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ۱. ثبت سرویس‌های لایه Application (منطق کسب‌وکار)
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductTypeService, ProductTypeService>();
        services.AddScoped<IFeatureService, FeatureService>();
        services.AddScoped<IFeatureValueService, FeatureValueService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<IProductService, ProductService>();

        // سرویس‌های ماژول Identity (جدید)
        services.AddScoped<IAuthService, AuthService>();

        // ۲. ثبت FluentValidation برای اعتبارسنجی خودکار DTOها
        services.AddValidatorsFromAssemblyContaining<CreateBrandValidator>();
        services.AddValidatorsFromAssemblyContaining<RequestOtpValidator>();
        services.AddFluentValidationAutoValidation();

        return services;
    }
}