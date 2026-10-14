using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Catalog;
using Domain.Constants;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت برچسب (Tag)
    /// </summary>
    public class TagService : ITagService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITagRepository _tagRepository;

        /// <summary>
        /// سازنده سرویس برچسب
        /// </summary>
        /// <param name="tagRepository">ریپازیتوری برچسب</param>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        public TagService(
            ITagRepository tagRepository,
            ICurrentUserService currentUserService)
        {
            _tagRepository = tagRepository;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// دریافت همه برچسب‌ها
        /// </summary>
        /// <returns>لیست تمام برچسب‌ها به صورت DTO</returns>
        public async Task<List<TagDto>> GetAllAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(MapToDto).ToList();
        }

        /// <summary>
        /// دریافت برچسب با شناسه
        /// </summary>
        /// <param name="id">شناسه برچسب</param>
        /// <returns>اطلاعات برچسب یا null در صورت عدم وجود</returns>
        public async Task<TagDto?> GetByIdAsync(long id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            return tag is null ? null : MapToDto(tag);
        }

        /// <summary>
        /// دریافت برچسب‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<TagDto>> GetPagedAsync(TagFilterDto filter)
        {
            var result = await _tagRepository.GetPagedAsync(filter);

            return new PagedResult<TagDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// ساخت برچسب جدید
        /// </summary>
        /// <param name="dto">اطلاعات برچسب جدید</param>
        /// <returns>برچسب ساخته شده به صورت DTO</returns>
        public async Task<TagDto> CreateAsync(CreateTagDto dto)
        {
            // ✅ بررسی تکراری نبودن نام (استفاده از ConflictException)
            if (await _tagRepository.ExistsByNameAsync(dto.Name.Trim()))
            {
                throw new ConflictException(
                    $"برچسب با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.Tag,
                    nameof(Tag.Name));
            }

            // ===== ساخت موجودیت =====
            var tag = new Tag(
                createBy: _currentUserService.UserId,
                name: dto.Name);

            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync();

            return MapToDto(tag);
        }

        /// <summary>
        /// به‌روزرسانی برچسب
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده</param>
        /// <returns>برچسب به‌روزرسانی شده به صورت DTO</returns>
        public async Task<TagDto> UpdateAsync(UpdateTagDto dto)
        {
            // ✅ بررسی وجود برچسب (استفاده از NotFoundException)
            var tag = await _tagRepository.GetByIdAsync(dto.Id);
            if (tag is null)
            {
                throw new NotFoundException(
                    $"برچسب با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.Tag,
                    nameof(Tag.Id));
            }

            // ✅ بررسی تکراری نبودن نام (به جز خودش) (استفاده از ConflictException)
            if (await _tagRepository.ExistsByNameAsync(dto.Name.Trim()))
            {
                if (tag.Name != dto.Name.Trim())
                {
                    throw new ConflictException(
                        $"برچسب با نام '{dto.Name}' قبلاً ثبت شده است.",
                        DomainConcept.Tag,
                        nameof(Tag.Name));
                }
            }

            // ===== به‌روزرسانی فیلدها =====
            tag.UpdateName(dto.Name, _currentUserService.UserId);

            // ===== تغییر وضعیت =====
            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    tag.Activate(_currentUserService.UserId);
                else if (status == EntityStatus.Inactive)
                    tag.Inactivate(_currentUserService.UserId);
            }

            _tagRepository.Update(tag);
            await _tagRepository.SaveChangesAsync();

            return MapToDto(tag);
        }

        /// <summary>
        /// حذف منطقی برچسب
        /// </summary>
        /// <param name="id">شناسه برچسب برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            // ✅ بررسی وجود برچسب قبل از حذف (استفاده از NotFoundException)
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag is null)
            {
                throw new NotFoundException(
                    $"برچسب با شناسه {id} یافت نشد.",
                    DomainConcept.Tag,
                    nameof(Tag.Id));
            }

            tag.Delete(_currentUserService.UserId);

            _tagRepository.Update(tag);
            await _tagRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تبدیل موجودیت به DTO
        /// </summary>
        /// <param name="tag">موجودیت برچسب</param>
        /// <returns>شیء DTO برچسب</returns>
        private static TagDto MapToDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Status = tag.Status.ToString(),
                CreateDate = tag.CreateDate,
                UpdateDate = tag.UpdateDate
            };
        }
    }
}