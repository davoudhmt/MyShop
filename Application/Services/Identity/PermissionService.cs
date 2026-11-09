using Application.DTOs.Common;
using Application.DTOs.Identity.Permission;
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
    /// پیاده‌سازی سرویس مدیریت مجوزها (Permission)
    /// این سرویس مسئولیت مدیریت چرخه حیات مجوزها، شامل ایجاد، به‌روزرسانی، حذف و دریافت لیست‌های گروه‌بندی شده را بر عهده دارد.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionRepository _permissionRepository;

        /// <summary>
        /// سازنده سرویس مجوز و مقداردهی وابستگی‌ها
        /// </summary>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        /// <param name="permissionRepository">ریپازیتوری مجوز</param>
        public PermissionService(
            ICurrentUserService currentUserService,
            IPermissionRepository permissionRepository)
        {
            _currentUserService = currentUserService;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// دریافت لیست صفحه‌بندی شده مجوزها بر اساس فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای جستجو و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<PermissionDto>> GetPagedAsync(PermissionFilterDto filter)
        {
            var pagedPermissions = await _permissionRepository.GetPagedAsync(filter);

            return new PagedResult<PermissionDto>
            {
                Items = pagedPermissions.Items.ToDtoList(),
                PageNumber = pagedPermissions.PageNumber,
                PageSize = pagedPermissions.PageSize,
                TotalCount = pagedPermissions.TotalCount
            };
        }

        /// <summary>
        /// دریافت مجوز با شناسه
        /// </summary>
        /// <param name="id">شناسه مجوز</param>
        /// <returns>اطلاعات مجوز یا null در صورت عدم وجود</returns>
        public async Task<PermissionDto?> GetByIdAsync(long id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            return permission?.ToDto();
        }

        /// <summary>
        /// ساخت مجوز جدید
        /// </summary>
        /// <param name="dto">اطلاعات مجوز جدید</param>
        /// <returns>مجوز ساخته شده به صورت DTO</returns>
        public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
        {
            if (await _permissionRepository.GetByNameAsync(dto.Name) is not null)
            {
                throw new ConflictException("نام مجوز تکراری است.", DomainConcept.Permission, nameof(dto.Name));
            }

            var permission = new Permission(
                createBy: _currentUserService.UserId,
                name: dto.Name,
                displayName: dto.DisplayName,
                module: dto.Module,
                description: dto.Description
            );

            await _permissionRepository.AddAsync(permission);
            await _permissionRepository.SaveChangesAsync();

            return permission.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات مجوز
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده مجوز</param>
        public async Task UpdateAsync(UpdatePermissionDto dto)
        {
            var permission = await _permissionRepository.GetByIdAsync(dto.Id);

            if (permission is null)
            {
                throw new NotFoundException($"مجوز با شناسه {dto.Id} یافت نشد.", DomainConcept.Permission, nameof(Permission.Id));
            }

            if (permission.Name != dto.Name && await _permissionRepository.GetByNameAsync(dto.Name) is not null)
            {
                throw new ConflictException("نام مجوز تکراری است.", DomainConcept.Permission, nameof(dto.Name));
            }

            permission.UpdateName(dto.Name, _currentUserService.UserId);
            permission.UpdateDisplayName(dto.DisplayName, _currentUserService.UserId);
            permission.UpdateModule(dto.Module, _currentUserService.UserId);
            permission.UpdateDescription(dto.Description, _currentUserService.UserId);

            _permissionRepository.Update(permission);
            await _permissionRepository.SaveChangesAsync();
        }

        /// <summary>
        /// حذف منطقی مجوز
        /// </summary>
        /// <param name="id">شناسه مجوز برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);

            if (permission is null)
            {
                throw new NotFoundException($"مجوز با شناسه {id} یافت نشد.", DomainConcept.Permission, nameof(Permission.Id));
            }

            permission.Delete(_currentUserService.UserId);

            _permissionRepository.Update(permission);
            await _permissionRepository.SaveChangesAsync();
        }

        /// <summary>
        /// فعال‌سازی مجوز
        /// </summary>
        /// <param name="id">شناسه مجوز برای فعال‌سازی</param>
        public async Task ActivateAsync(long id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission is null)
                throw new NotFoundException($"مجوز با شناسه {id} یافت نشد.", DomainConcept.Permission, nameof(Permission.Id));

            permission.Activate(_currentUserService.UserId);
            _permissionRepository.Update(permission);
            await _permissionRepository.SaveChangesAsync();
        }

        /// <summary>
        /// غیرفعال‌سازی مجوز
        /// </summary>
        /// <param name="id">شناسه مجوز برای غیرفعال‌سازی</param>
        public async Task DeactivateAsync(long id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission is null)
                throw new NotFoundException($"مجوز با شناسه {id} یافت نشد.", DomainConcept.Permission, nameof(Permission.Id));

            permission.Deactivate(_currentUserService.UserId);
            _permissionRepository.Update(permission);
            await _permissionRepository.SaveChangesAsync();
        }

        /// <summary>
        /// دریافت مجوزها به صورت گروه‌بندی شده بر اساس ماژول
        /// این متد برای نمایش در پنل ادمین استفاده می‌شود
        /// </summary>
        /// <returns>دیکشنری که کلید آن ماژول و مقدار آن لیست DTOهای مجوزهای آن ماژول است</returns>
        public async Task<IReadOnlyDictionary<Domain.Enums.Module, IReadOnlyList<PermissionDto>>> GetGroupedByModuleAsync()
        {
            var groupedEntities = await _permissionRepository.GetGroupedByModuleAsync();

            return groupedEntities.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<PermissionDto>)kvp.Value.ToDtoList()
            );
        }
    }
}