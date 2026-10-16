using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Catalog;
using Application.Mappings.Catalog;
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
            return tags.ToDtoList();
        }

        /// <summary>
        /// دریافت برچسب با شناسه
        /// </summary>
        /// <param name="id">شناسه برچسب</param>
        /// <returns>اطلاعات برچسب یا null در صورت عدم وجود</returns>
        public async Task<TagDto?> GetByIdAsync(long id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            return tag?.ToDto();
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
                Items = result.Items.ToDtoList(),
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
            if (await _tagRepository.ExistsByNameAsync(dto.Name.Trim()))
            {
                throw new ConflictException(
                    $"برچسب با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.Tag,
                    nameof(Tag.Name));
            }

            var tag = new Tag(
                createBy: _currentUserService.UserId,
                name: dto.Name);

            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync();

            return tag.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی برچسب
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده</param>
        /// <returns>برچسب به‌روزرسانی شده به صورت DTO</returns>
        public async Task<TagDto> UpdateAsync(UpdateTagDto dto)
        {
            var tag = await _tagRepository.GetByIdAsync(dto.Id);
            if (tag is null)
            {
                throw new NotFoundException(
                    $"برچسب با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.Tag,
                    nameof(Tag.Id));
            }

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

            tag.UpdateName(dto.Name, _currentUserService.UserId);

            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    tag.Activate(_currentUserService.UserId);
                else if (status == EntityStatus.Inactive)
                    tag.Inactivate(_currentUserService.UserId);
            }

            _tagRepository.Update(tag);
            await _tagRepository.SaveChangesAsync();

            return tag.ToDto();
        }

        /// <summary>
        /// حذف منطقی برچسب
        /// </summary>
        /// <param name="id">شناسه برچسب برای حذف</param>
        public async Task DeleteAsync(long id)
        {
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
    }
}