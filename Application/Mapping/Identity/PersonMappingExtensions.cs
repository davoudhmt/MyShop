using Application.DTOs.Identity.Person;
using Application.DTOs.Identity.User;
using Domain.Entities.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Application.Mappings.Identity
{
    /// <summary>
    /// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت Person به DTOهای مختلف.
    /// </summary>
    public static class PersonMappingExtensions
    {
        // ==========================================
        // ۱. نگاشت پیش‌فرض (مخصوص کاربر عادی و AuthService)
        // ==========================================

        /// <summary>
        /// تبدیل موجودیت Person به شیء UserProfileDto (برای نمایش پروفایل خودِ کاربر)
        /// </summary>
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

        // ==========================================
        // ۲. نگاشت مخصوص پنل مدیریت (ادمین)
        // ==========================================

        /// <summary>
        /// تبدیل موجودیت Person به شیء PersonDto (برای نمایش در لیست و مدیریت پنل ادمین)
        /// </summary>
        public static PersonDto ToAdminDto(this Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                FullName = person.FullName,
                Mobile = person.Mobile.Value,
                Landline = person.Landline?.Value,
                Email = person.Email?.Value,
                HasUserAccount = person.UserId.HasValue,
                Status = person.Status.ToString(),
                CreateDate = person.CreateDate
            };
        }

        /// <summary>
        /// تبدیل لیستی از موجودیت‌های Person به لیست PersonDto (برای صفحه‌بندی در پنل ادمین)
        /// </summary>
        public static List<PersonDto> ToAdminDtoList(this IEnumerable<Person> persons)
        {
            return persons.Select(ToAdminDto).ToList();
        }
    }
}