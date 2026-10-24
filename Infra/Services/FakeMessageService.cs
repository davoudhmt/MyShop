using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Infra.Services
{
    /// <summary>
    /// پیاده‌سازی موقت سرویس پیام برای اهداف توسعه و تست
    /// این کلاس پیام را به جای ارسال واقعی، در لاگ سیستم ثبت می‌کند.
    /// </summary>
    public class FakeMessageService : IMessageService
    {
        private readonly ILogger<FakeMessageService> _logger;

        /// <summary>
        /// سازنده سرویس پیام موقت
        /// </summary>
        /// <param name="logger">سرویس لاگر برای ثبت پیام‌ها در کنسول</param>
        public FakeMessageService(ILogger<FakeMessageService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// شبیه‌سازی ارسال پیام به شماره موبایل
        /// </summary>
        public Task SendToMobileAsync(string mobile, string message)
        {
            // ثبت پیام در لاگ به جای ارسال واقعی
            _logger.LogWarning("📩 [SIMULATED SMS] To: {Mobile} | Message: {Message}", mobile, message);

            return Task.CompletedTask;
        }
    }
}