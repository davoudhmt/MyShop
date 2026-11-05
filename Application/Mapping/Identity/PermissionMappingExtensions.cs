using Application.DTOs.Identity.Permission;
using Domain.Entities.Catalog;
using Domain.Entities.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Application.Mappings.Identity
{
    /// <summary>
    /// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت Permission به DTO و بالعکس.
    /// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود.
    /// </summary>
    public static class PermissionMappingExtensions
    {
        /// <summary>
        /// تبدیل موجودیت Permission به شیء PermissionDto
        /// </summary>
        /// <param name="permission">موجودیت مجوز</param>
        /// <returns>شیء DTO مجوز</returns>
        public static PermissionDto ToDto(this Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                DisplayName = permission.DisplayName,
                Module = permission.Module,
                Description = permission.Description,
                CreateDate = permission.CreateDate,
                Status = permission.Status.ToString(),
                UpdateDate = permission.UpdateDate
            };
        }

        /// <summary>
        /// تبدیل لیستی از موجودیت‌های Permission به لیست PermissionDto
        /// </summary>
        /// <param name="permissions">لیست موجودیت‌های مجوز</param>
        /// <returns>لیست DTOهای مجوز</returns>
        public static List<PermissionDto> ToDtoList(this IEnumerable<Permission> permissions)
        {
            return permissions.Select(ToDto).ToList();
        }
    }
}