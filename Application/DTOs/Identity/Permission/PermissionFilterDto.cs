using Application.DTOs.Common;

namespace Application.DTOs.Identity.Permission
{
    /// <summary>
    /// DTO برای فیلتر، جستجو و صفحه‌بندی مجوزهای سیستم
    /// </summary>
    public class PermissionFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام (Name) یا نام نمایشی (DisplayName) مجوز (اختیاری)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر وضعیت:
        /// - خالی یا "All": همه مجوزها
        /// - "Active": فقط مجوزهای فعال
        /// - "Inactive": فقط مجوزهای غیرفعال
        /// </summary>
        public string? Status { get; set; }
    }
}