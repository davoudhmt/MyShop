using Application.Interfaces.Services;
using System.Security.Claims;

namespace API.Services
{
    /// <summary>
    /// پیاده‌سازی سرویس دسترسی به اطلاعات کاربر فعلی از طریق HttpContext
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// سازنده سرویس کاربر فعلی
        /// </summary>
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// شناسه عددی کاربر فعلی (استخراج شده از Claim استاندارد NameIdentifier)
        /// </summary>
        public long UserId
        {
            get
            {
                var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    // اگر کاربر لاگین نکرده باشد، فعلاً 0 برمی‌گردانیم 
                    // (در آینده می‌توانید اینجا استثنا پرتاب کنید یا اجازه دسترسی ندهید)
                    return 0;
                }

                return userId;
            }
        }

        /// <summary>
        /// بررسی احراز هویت بودن کاربر
        /// </summary>
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}