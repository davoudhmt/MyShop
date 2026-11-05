using Application.DTOs.Identity.Permission;
using Application.DTOs.Identity.Role;
using Domain.Entities.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Application.Mappings.Identity
{
    /// <summary>
    /// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت Role به DTO و بالعکس.
    /// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود.
    /// </summary>
    public static class RoleMappingExtensions
    {
        /// <summary>
        /// تبدیل موجودیت Role به شیء RoleDto (مناسب برای لیست و گریدها)
        /// </summary>
        /// <param name="role">موجودیت نقش</param>
        /// <returns>شیء DTO خلاصه نقش</returns>
        public static RoleDto ToDto(this Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsSystem = role.IsSystem,
                CreateDate = role.CreateDate,
                Status = role.Status.ToString()
            };
        }

        /// <summary>
        /// تبدیل لیستی از موجودیت‌های Role به لیست RoleDto
        /// </summary>
        /// <param name="roles">لیست موجودیت‌های نقش</param>
        /// <returns>لیست DTOهای خلاصه نقش</returns>
        public static List<RoleDto> ToDtoList(this IEnumerable<Role> roles)
        {
            return roles.Select(ToDto).ToList();
        }

        /// <summary>
        /// تبدیل موجودیت Role به شیء RoleDetailsDto (شامل لیست مجوزها)
        /// نکته: این متد فرض می‌کند که Navigation Property های RolePermissions و Permission در EF Core لود شده‌اند (Include شده‌اند).
        /// </summary>
        /// <param name="role">موجودیت نقش</param>
        /// <returns>شیء DTO جزئیات نقش به همراه مجوزها</returns>
        public static RoleDetailsDto ToDetailsDto(this Role role)
        {
            return new RoleDetailsDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsSystem = role.IsSystem,
                CreateDate = role.CreateDate,
                Status = role.Status.ToString(),

                // مپ کردن مجوزهای تخصیص‌یافته از طریق جدول واسط
                Permissions = role.RolePermissions
                    .Select(rp => rp.Permission)
                    .ToDtoList()
                    .AsReadOnly()
            };
        }
    }
}