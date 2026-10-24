using API.Services;
using Application.Interfaces.Services;

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
        // ثبت سرویس تولید توکن (پیاده‌سازی JWT)
        services.AddScoped<ITokenService, JwtTokenService>();

        // این سرویس فقط مخصوص محیط توسعه/ارائه است و جایگاهش همینجاست
        //services.AddScoped<ICurrentUserService, MockCurrentUserService>();

        // ثبت سرویس واقعی برای دریافت اطلاعات کاربر جاری از توکن JWT
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}