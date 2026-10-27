using Application.Interfaces.Services.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services
{
    /// <summary>
    /// پیاده‌سازی سرویس کش OTP با استفاده از IMemoryCache (حافظه موقت درون‌برنامه‌ای)
    /// این پیاده‌سازی برای محیط توسعه و تست مناسب است و به راحتی با Redis قابل جایگزینی است
    /// </summary>
    public class MemoryOtpCacheService : IOtpCacheService
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// سازنده سرویس کش OTP با تزریق IMemoryCache
        /// </summary>
        /// <param name="cache">سرویس کش حافظه‌ای</param>
        public MemoryOtpCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// ذخیره کد تأیید در کش با زمان انقضای مشخص
        /// </summary>
        public Task SetOtpAsync(string mobile, string otpCode, TimeSpan expiration)
        {
            var cacheKey = GetCacheKey(mobile);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = null
            };

            _cache.Set(cacheKey, otpCode, cacheEntryOptions);

            return Task.CompletedTask;
        }

        /// <summary>
        /// دریافت کد تأیید از کش بر اساس شماره موبایل
        /// </summary>
        public Task<string?> GetOtpAsync(string mobile)
        {
            var cacheKey = GetCacheKey(mobile);

            _cache.TryGetValue(cacheKey, out string? otpCode);

            return Task.FromResult(otpCode);
        }

        /// <summary>
        /// حذف کد تأیید از کش (پس از استفاده موفق)
        /// </summary>
        public Task RemoveOtpAsync(string mobile)
        {
            var cacheKey = GetCacheKey(mobile);

            _cache.Remove(cacheKey);

            return Task.CompletedTask;
        }

        /// <summary>
        /// تولید کلید کش یکنواخت برای شماره موبایل
        /// </summary>
        private static string GetCacheKey(string mobile)
        {
            return $"otp:{mobile}";
        }
    }
}