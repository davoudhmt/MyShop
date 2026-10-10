using Application.Interfaces.Services;

namespace API.Services
{
    /// <summary>
    /// نسخه موقت سرویس کاربر فعلی (تا زمان پیاده‌سازی سیستم احراز هویت)
    /// </summary>
    public class MockCurrentUserService : ICurrentUserService
    {
        /// <summary>
        /// شناسه کاربر پیش‌فرض (موقت)
        /// </summary>
        public long UserId => 1;

        /// <summary>
        /// در حالت موقت، همیشه احراز هویت شده در نظر گرفته می‌شود
        /// </summary>
        public bool IsAuthenticated => true;
    }
}