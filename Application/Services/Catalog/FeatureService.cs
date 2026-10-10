using Application.DTOs.Catalog.Feature;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس ویژگی
    /// </summary>
    public class FeatureService : IFeatureService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IFeatureRepository _featureRepository;

        /// <summary>
        /// سازنده سرویس ویژگی
        /// </summary>
        /// <param name="featureRepository">ریپازیتوری ویژگی</param>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        public FeatureService(
            IFeatureRepository featureRepository,
            ICurrentUserService currentUserService)
        {
            _featureRepository = featureRepository;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// دریافت همه ویژگی‌ها
        /// </summary>
        public async Task<List<FeatureDto>> GetAllAsync()
        {
            var features = await _featureRepository.GetAllAsync();
            return features.Select(ToDto).ToList();
        }

        /// <summary>
        /// دریافت ویژگی با شناسه
        /// </summary>
        public async Task<FeatureDto?> GetByIdAsync(long id)
        {
            var feature = await _featureRepository.GetByIdAsync(id);
            return feature is null ? null : ToDto(feature);
        }

        /// <summary>
        /// دریافت ویژگی‌های یک نوع محصول خاص
        /// </summary>
        public async Task<List<FeatureDto>> GetByProductTypeIdAsync(long productTypeId)
        {
            var features = await _featureRepository.GetByProductTypeIdAsync(productTypeId);
            return features.Select(ToDto).ToList();
        }

        /// <summary>
        /// دریافت ویژگی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<FeatureDto>> GetPagedAsync(FeatureFilterDto filter)
        {
            var result = await _featureRepository.GetPagedAsync(filter);

            return new PagedResult<FeatureDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// ساخت ویژگی جدید
        /// </summary>
        public async Task<FeatureDto> CreateAsync(CreateFeatureDto dto)
        {
            // ===== بررسی تکراری نبودن نام =====
            if (await _featureRepository.ExistsByNameAsync(dto.Name.Trim()))
                throw new InvalidOperationException($"ویژگی با نام '{dto.Name}' قبلاً ثبت شده است.");

            // ===== ساخت موجودیت =====
            var feature = new Feature(
                createBy: _currentUserService.UserId,
                name: dto.Name,
                multiplicity: dto.Multiplicity);

            await _featureRepository.AddAsync(feature);
            await _featureRepository.SaveChangesAsync();

            return ToDto(feature);
        }

        /// <summary>
        /// به‌روزرسانی ویژگی
        /// </summary>
        public async Task<FeatureDto> UpdateAsync(UpdateFeatureDto dto)
        {
            var feature = await _featureRepository.GetByIdAsync(dto.Id);
            if (feature is null)
                throw new InvalidOperationException($"ویژگی با شناسه {dto.Id} یافت نشد.");

            // ===== بررسی تکراری نبودن نام (به جز خودش) =====
            var existingByName = await _featureRepository.GetByNameAsync(dto.Name.Trim());
            if (existingByName is not null && existingByName.Id != dto.Id)
                throw new InvalidOperationException($"ویژگی با نام '{dto.Name}' قبلاً ثبت شده است.");

            // ===== به‌روزرسانی فیلدها =====
            feature.UpdateFeatureName(dto.Name, _currentUserService.UserId);
            feature.UpdateFeatureMultiplicity(dto.Multiplicity, _currentUserService.UserId);

            // ===== تغییر وضعیت =====
            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    feature.Activate(_currentUserService.UserId);
                else if (status == EntityStatus.Inactive)
                    feature.Inactivate(_currentUserService.UserId);
            }

            _featureRepository.Update(feature);
            await _featureRepository.SaveChangesAsync();

            return ToDto(feature);
        }

        /// <summary>
        /// حذف منطقی ویژگی
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            var feature = await _featureRepository.GetByIdAsync(id);
            if (feature is null)
                throw new InvalidOperationException($"ویژگی با شناسه {id} یافت نشد.");

            feature.Delete(_currentUserService.UserId);

            _featureRepository.Update(feature);
            await _featureRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تبدیل موجودیت به DTO
        /// </summary>
        private static FeatureDto ToDto(Feature feature)
        {
            return new FeatureDto
            {
                Id = feature.Id,
                Name = feature.Name,
                Multiplicity = feature.Multiplicity.ToString(),
                Status = feature.Status.ToString(),
                CreateDate = feature.CreateDate,
                UpdateDate = feature.UpdateDate
            };
        }
    }
}