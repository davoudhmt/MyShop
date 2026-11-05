using Application.DTOs.Common;

namespace Application.DTOs.Identity.Role
{
    /// <summary>
    /// DTO برای فیلتر، جستجو و صفحه‌بندی نقش‌های سیستم
    /// </summary>
    public class RoleFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو در نام یا توضیحات نقش (اختیاری)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر وضعیت:
        /// - خالی یا "All": همه نقش‌ها
        /// - "Active": فقط نقش‌های فعال
        /// - "Inactive": فقط نقش‌های غیرفعال
        /// </summary>
        public string? Status { get; set; }
    }
}