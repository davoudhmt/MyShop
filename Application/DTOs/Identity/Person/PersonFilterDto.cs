using Application.DTOs.Common;

namespace Application.DTOs.Identity.Person
{
    /// <summary>
    /// DTO برای فیلتر، جستجو و صفحه‌بندی اشخاص در پنل مدیریت
    /// </summary>
    public class PersonFilterDto : PagingParams
    {
        /// <summary>
        /// عبارت جستجو (جستجو در نام، نام خانوادگی یا شماره موبایل)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// فیلتر وضعیت (Active, Inactive, Deleted, All)
        /// </summary>
        public string? Status { get; set; }
    }
}