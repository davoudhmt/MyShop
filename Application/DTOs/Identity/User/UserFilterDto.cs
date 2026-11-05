using Application.DTOs.Common;

namespace Application.DTOs.Identity.User
{
    /// <summary>
    /// DTO برای فیلتر، جستجو و صفحه‌بندی کاربران در پنل مدیریت
    /// </summary>
    public class UserFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام کاربری (شماره موبایل)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر وضعیت:
        /// - خالی یا "All": همه کاربران
        /// - "Active": فقط کاربران فعال
        /// - "Inactive": فقط کاربران غیرفعال
        /// </summary>
        public string? Status { get; set; }
    }
}