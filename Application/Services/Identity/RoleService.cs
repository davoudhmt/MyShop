using Application.DTOs.Common;
using Application.DTOs.Identity.Role;
using Application.Interfaces.Repositories.Identity;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Mappings.Identity;
using Domain.Constants;
using Domain.Entities.Identity;
using Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Identity
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت نقش‌ها (Role)
    /// این سرویس مسئولیت مدیریت چرخه حیات نقش‌ها، شامل ایجاد، به‌روزرسانی، حذف و دریافت جزئیات را بر عهده دارد.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// سازنده سرویس نقش و مقداردهی وابستگی‌ها
        /// </summary>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        /// <param name="roleRepository">ریپازیتوری نقش</param>
        public RoleService(
            ICurrentUserService currentUserService,
            IRoleRepository roleRepository)
        {
            _currentUserService = currentUserService;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// دریافت لیست صفحه‌بندی شده نقش‌ها بر اساس فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای جستجو و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<RoleDto>> GetPagedAsync(RoleFilterDto filter)
        {
            var pagedRoles = await _roleRepository.GetPagedAsync(filter);

            return new PagedResult<RoleDto>
            {
                Items = pagedRoles.Items.ToDtoList(),
                PageNumber = pagedRoles.PageNumber,
                PageSize = pagedRoles.PageSize,
                TotalCount = pagedRoles.TotalCount
            };
        }

        /// <summary>
        /// دریافت نقش با شناسه (خلاصه)
        /// </summary>
        /// <param name="id">شناسه نقش</param>
        /// <returns>اطلاعات خلاصه نقش یا null در صورت عدم وجود</returns>
        public async Task<RoleDto?> GetByIdAsync(long id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role?.ToDto();
        }

        /// <summary>
        /// دریافت جزئیات کامل نقش به همراه لیست مجوزهای تخصیص‌یافته
        /// </summary>
        /// <param name="id">شناسه نقش</param>
        /// <returns>جزئیات نقش به همراه مجوزها یا null در صورت عدم وجود</returns>
        public async Task<RoleDetailsDto?> GetDetailsAsync(long id)
        {
            var role = await _roleRepository.GetDetailsAsync(id);
            return role?.ToDetailsDto();
        }

        /// <summary>
        /// ساخت نقش جدید
        /// </summary>
        /// <param name="dto">اطلاعات نقش جدید</param>
        /// <returns>نقش ساخته شده به صورت DTO</returns>
        public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
        {
            if (await _roleRepository.GetByNameAsync(dto.Name) is not null)
            {
                throw new ConflictException("نام نقش تکراری است.", DomainConcept.Role, nameof(dto.Name));
            }

            var role = new Role(
                createBy: _currentUserService.UserId,
                name: dto.Name,
                isSystem: dto.IsSystem,
                description: dto.Description
            );

            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();

            return role.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات نقش
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده نقش</param>
        public async Task UpdateAsync(UpdateRoleDto dto)
        {
            var role = await _roleRepository.GetByIdAsync(dto.Id);

            if (role is null)
            {
                throw new NotFoundException($"نقش با شناسه {dto.Id} یافت نشد.", DomainConcept.Role, nameof(Role.Id));
            }

            if (role.Name != dto.Name && await _roleRepository.GetByNameAsync(dto.Name) is not null)
            {
                throw new ConflictException("نام نقش تکراری است.", DomainConcept.Role, nameof(dto.Name));
            }

            role.UpdateName(dto.Name, _currentUserService.UserId);
            role.UpdateDescription(dto.Description, _currentUserService.UserId);

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }

        /// <summary>
        /// حذف منطقی نقش
        /// </summary>
        /// <param name="id">شناسه نقش برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role is null)
            {
                throw new NotFoundException($"نقش با شناسه {id} یافت نشد.", DomainConcept.Role, nameof(Role.Id));
            }

            role.Delete(_currentUserService.UserId);

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }

        /// <summary>
        /// فعال‌سازی نقش
        /// </summary>
        /// <param name="id">شناسه نقش برای فعال‌سازی</param>
        public async Task ActivateAsync(long id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role is null)
            {
                throw new NotFoundException($"نقش با شناسه {id} یافت نشد.", DomainConcept.Role, nameof(Role.Id));
            }

            role.Activate(_currentUserService.UserId);

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }

        /// <summary>
        /// غیرفعال‌سازی نقش
        /// </summary>
        /// <param name="id">شناسه نقش برای غیرفعال‌سازی</param>
        public async Task DeactivateAsync(long id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role is null)
            {
                throw new NotFoundException($"نقش با شناسه {id} یافت نشد.", DomainConcept.Role, nameof(Role.Id));
            }

            role.Deactivate(_currentUserService.UserId);

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }
    }
}