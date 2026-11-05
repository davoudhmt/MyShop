using Application.DTOs.Identity.User;
using Domain.Entities.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Application.Mappings.Identity
{
    /// <summary>
    /// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت Person به DTO و بالعکس.
    /// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود.
    /// </summary>
    public static class PersonMappingExtensions
    {
        /// <summary>
        /// تبدیل موجودیت Person به شیء UserProfileDto
        /// </summary>
        /// <param name="person">موجودیت شخص</param>
        /// <returns>شیء DTO پروفایل کاربر</returns>
        public static UserProfileDto ToDto(this Person person)
        {
            return new UserProfileDto
            {
                PersonId = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Mobile = person.Mobile.Value,
                Email = person.Email?.Value,
                Status = person.Status.ToString(),
                CreateDate = person.CreateDate,
                UpdateDate = person.UpdateDate
            };
        }

        /// <summary>
        /// تبدیل لیستی از موجودیت‌های Person به لیست UserProfileDto
        /// </summary>
        /// <param name="persons">لیست موجودیت‌های شخص</param>
        /// <returns>لیست DTOهای پروفایل کاربر</returns>
        public static List<UserProfileDto> ToDtoList(this IEnumerable<Person> persons)
        {
            return persons.Select(ToDto).ToList();
        }
    }
}