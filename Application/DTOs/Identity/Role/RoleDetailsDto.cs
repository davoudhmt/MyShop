using System;
using System.Collections.Generic;
using Application.DTOs.Identity.Permission;

namespace Application.DTOs.Identity.Role
{
    /// <summary>
    /// DTO برای نمایش جزئیات کامل نقش به همراه لیست مجوزهای تخصیص‌یافته
    /// (مناسب برای صفحه ویرایش نقش یا نمایش هنگام تخصیص نقش به کاربر)
    /// </summary>
    public class RoleDetailsDto : RoleDto
    {
        /// <summary>
        /// لیست مجوزهای (Permissions) تخصیص‌یافته به این نقش
        /// </summary>
        public IReadOnlyList<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}