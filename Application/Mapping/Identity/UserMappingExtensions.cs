using Application.DTOs.Identity.User;
using Domain.Entities.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Application.Mappings.Identity
{
    /// <summary>
    /// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت User به DTO و بالعکس.
    /// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود.
    /// </summary>
    public static class UserMappingExtensions
    {
        /// <summary>
        /// تبدیل موجودیت User به شیء UserDto (مناسب برای لیست و مدیریت در پنل ادمین)
        /// </summary>
        /// <param name="user">موجودیت کاربر</param>
        /// <returns>شیء DTO کاربر</returns>
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                PersonId = user.PersonId,
                Username = user.Username,
                LastLoginDate = user.LastLoginDate,
                LastLoginIp = user.LastLoginIp,
                TwoFactorEnabled = user.TwoFactorEnabled,
                AccessFailedCount = user.AccessFailedCount,
                LockoutEnd = user.LockoutEnd,
                Status = user.Status.ToString(),
                CreateDate = user.CreateDate,
                UpdateDate = user.UpdateDate
            };
        }

        /// <summary>
        /// تبدیل لیستی از موجودیت‌های User به لیست UserDto
        /// </summary>
        /// <param name="users">لیست موجودیت‌های کاربر</param>
        /// <returns>لیست DTOهای کاربر</returns>
        public static List<UserDto> ToDtoList(this IEnumerable<User> users)
        {
            return users.Select(ToDto).ToList();
        }
    }
}