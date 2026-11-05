using Application.DTOs.Common;
using Application.DTOs.Identity.User;
using Application.Interfaces.Repositories.Identity;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Mappings.Identity;
using Domain.Constants;
using Domain.Entities.Identity;
using Domain.Exceptions;
using System.Threading.Tasks;

namespace Application.Services.Identity
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت کاربران (User)
    /// این سرویس مسئولیت مدیریت چرخه حیات کاربران، شامل دریافت لیست، جزئیات و حذف منطقی را بر عهده دارد.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// سازنده سرویس کاربر و مقداردهی وابستگی‌ها
        /// </summary>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        /// <param name="userRepository">ریپازیتوری کاربر</param>
        public UserService(
            ICurrentUserService currentUserService,
            IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// دریافت لیست صفحه‌بندی شده کاربران بر اساس فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای جستجو و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<UserDto>> GetPagedAsync(UserFilterDto filter)
        {
            var pagedUsers = await _userRepository.GetPagedAsync(filter);

            return new PagedResult<UserDto>
            {
                Items = pagedUsers.Items.ToDtoList(),
                PageNumber = pagedUsers.PageNumber,
                PageSize = pagedUsers.PageSize,
                TotalCount = pagedUsers.TotalCount
            };
        }

        /// <summary>
        /// دریافت کاربر با شناسه
        /// </summary>
        /// <param name="id">شناسه کاربر</param>
        /// <returns>اطلاعات کاربر یا null در صورت عدم وجود</returns>
        public async Task<UserDto?> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.ToDto();
        }

        /// <summary>
        /// حذف منطقی کاربر
        /// </summary>
        /// <param name="id">شناسه کاربر برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"کاربر با شناسه {id} یافت نشد.", DomainConcept.User, nameof(User.Id));
            }

            // جلوگیری از حذف کاربر جاری توسط خودش
            if (user.Id == _currentUserService.UserId)
            {
                throw new DomainException("امکان حذف کاربر جاری وجود ندارد.", DomainConcept.User, nameof(User.Id));
            }

            user.Delete(_currentUserService.UserId);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}