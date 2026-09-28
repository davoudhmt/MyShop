using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس برچسب
    /// </summary>
    public class TagService : ITagService
    {
        /// <summary>
        /// شناسه کاربر سیستم (موقتاً ثابت)
        /// </summary>
        private const long SystemUser = 1;

        private readonly ITagRepository _tagRepository;

        /// <summary>
        /// سازنده سرویس برچسب
        /// </summary>
        /// <param name="tagRepository">ریپازیتوری برچسب</param>
        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        /// <summary>
        /// دریافت همه برچسب‌ها
        /// </summary>
        public async Task<List<TagDto>> GetAllAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(MapToDto).ToList();
        }

        /// <summary>
        /// دریافت برچسب با شناسه
        /// </summary>
        public async Task<TagDto?> GetByIdAsync(long id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            return tag is null ? null : MapToDto(tag);
        }

        /// <summary>
        /// دریافت برچسب‌ها با صفحه‌بندی و فیلتر
        /// </summary>
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
        public async Task<TagDto> CreateAsync(CreateTagDto dto)
        {
            // ===== بررسی تکراری نبودن نام =====
            if (await _tagRepository.ExistsByNameAsync(dto.Name.Trim()))
                throw new InvalidOperationException($"برچسب با نام '{dto.Name}' قبلاً ثبت شده است.");

            // ===== ساخت موجودیت =====
            var tag = new Tag(
                createBy: SystemUser,
                name: dto.Name);

            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync();

            return MapToDto(tag);
        }

        /// <summary>
        /// به‌روزرسانی برچسب
        /// </summary>
        public async Task<TagDto> UpdateAsync(UpdateTagDto dto)
        {
            var tag = await _tagRepository.GetByIdAsync(dto.Id);
            if (tag is null)
                throw new InvalidOperationException($"برچسب با شناسه {dto.Id} یافت نشد.");

            // ===== بررسی تکراری نبودن نام (به جز خودش) =====
            if (await _tagRepository.ExistsByNameAsync(dto.Name.Trim()))
            {
                if (tag.Name != dto.Name.Trim())
                    throw new InvalidOperationException($"برچسب با نام '{dto.Name}' قبلاً ثبت شده است.");
            }

            // ===== به‌روزرسانی فیلدها =====
            tag.UpdateName(dto.Name, SystemUser);

            // ===== تغییر وضعیت =====
            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    tag.Activate(SystemUser);
                else if (status == EntityStatus.Inactive)
                    tag.Inactivate(SystemUser);
            }

            _tagRepository.Update(tag);
            await _tagRepository.SaveChangesAsync();

            return MapToDto(tag);
        }

        /// <summary>
        /// حذف منطقی برچسب
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag is null)
                throw new InvalidOperationException($"برچسب با شناسه {id} یافت نشد.");

            tag.Delete(SystemUser);

            _tagRepository.Update(tag);
            await _tagRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تبدیل موجودیت به DTO
        /// </summary>
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