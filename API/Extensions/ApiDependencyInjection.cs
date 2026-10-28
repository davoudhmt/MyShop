using API.Services;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;

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

        // این سرویس فقط مخصوص محیط توسعه/ارائه است
        //services.AddScoped<ICurrentUserService, MockCurrentUserService>();

        // ثبت سرویس واقعی برای دریافت اطلاعات کاربر جاری از توکن JWT
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ثبت سرویس کش OTP با استفاده از IMemoryCache (حافظه موقت درون‌برنامه‌ای)
        services.AddSingleton<IOtpCacheService, MemoryOtpCacheService>();

        // ثبت سرویس هش کردن رمز عبور
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        return services;
    }
}