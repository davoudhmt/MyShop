using Application.DTOs.Identity.User;
using Domain.Entities.Identity;

namespace Application.Mappings.Identity
{
    /// <summary>
    /// متدهای توسعه‌دهنده برای تبدیل موجودیت Person به DTO
    /// </summary>
    public static class PersonMappingExtensions
    {
        /// <summary>
        /// تبدیل موجودیت Person به شیء UserProfileDto
        /// </summary>
        public static UserProfileDto ToDto(this Person person)
        {
            return new UserProfileDto
            {
                PersonId = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Mobile = person.Mobile.Value,
                Email = person.Email?.Value
            };
        }
    }
}