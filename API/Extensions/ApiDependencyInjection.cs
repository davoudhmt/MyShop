using API.Services;
using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions;

/// <summary>
/// کلاس توسعه‌دهنده برای ثبت وابستگی‌های مخصوص لایه‌ی ارائه (API)
/// </summary>
public static class ApiDependencyInjection
{
    /// <summary>
    /// ثبت سرویس‌های خاص لایه API (مانند سرویس‌های موقت، Middlewareهای خاص و غیره)
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // ثبت سرویس موقت کاربر (تا زمان پیاده‌سازی سیستم احراز هویت واقعی)
        // این سرویس فقط مخصوص محیط توسعه/ارائه است و جایگاهش همینجاست
        services.AddScoped<ICurrentUserService, MockCurrentUserService>();

        return services;
    }
}