using Application.DTOs.Catalog.Feature;
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
    /// پیاده‌سازی سرویس مدیریت ویژگی‌ها (Features)
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
        /// <returns>لیست تمام ویژگی‌ها به صورت DTO</returns>
        public async Task<List<FeatureDto>> GetAllAsync()
        {
            var features = await _featureRepository.GetAllAsync();
            return features.ToDtoList();
        }

        /// <summary>
        /// دریافت ویژگی با شناسه
        /// </summary>
        /// <param name="id">شناسه ویژگی</param>
        /// <returns>اطلاعات ویژگی یا null در صورت عدم وجود</returns>
        public async Task<FeatureDto?> GetByIdAsync(long id)
        {
            var feature = await _featureRepository.GetByIdAsync(id);
            return feature?.ToDto();
        }

        /// <summary>
        /// دریافت ویژگی‌های یک نوع محصول خاص
        /// </summary>
        /// <param name="productTypeId">شناسه نوع محصول</param>
        /// <returns>لیست ویژگی‌های مرتبط به صورت DTO</returns>
        public async Task<List<FeatureDto>> GetByProductTypeIdAsync(long productTypeId)
        {
            var features = await _featureRepository.GetByProductTypeIdAsync(productTypeId);
            return features.ToDtoList();
        }

        /// <summary>
        /// دریافت ویژگی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<FeatureDto>> GetPagedAsync(FeatureFilterDto filter)
        {
            var result = await _featureRepository.GetPagedAsync(filter);

            return new PagedResult<FeatureDto>
            {
                Items = result.Items.ToDtoList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// ساخت ویژگی جدید
        /// </summary>
        /// <param name="dto">اطلاعات ویژگی جدید</param>
        /// <returns>ویژگی ساخته شده به صورت DTO</returns>
        public async Task<FeatureDto> CreateAsync(CreateFeatureDto dto)
        {
            if (await _featureRepository.ExistsByNameAsync(dto.Name.Trim()))
            {
                throw new ConflictException(
                    $"ویژگی با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.Feature,
                    nameof(Feature.Name));
            }

            var feature = new Feature(
                createBy: _currentUserService.UserId,
                name: dto.Name,
                multiplicity: dto.Multiplicity);

            await _featureRepository.AddAsync(feature);
            await _featureRepository.SaveChangesAsync();

            return feature.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی ویژگی
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده</param>
        /// <returns>ویژگی به‌روزرسانی شده به صورت DTO</returns>
        public async Task<FeatureDto> UpdateAsync(UpdateFeatureDto dto)
        {
            var feature = await _featureRepository.GetByIdAsync(dto.Id);
            if (feature is null)
            {
                throw new NotFoundException(
                    $"ویژگی با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.Feature,
                    nameof(Feature.Id));
            }

            var existingByName = await _featureRepository.GetByNameAsync(dto.Name.Trim());
            if (existingByName is not null && existingByName.Id != dto.Id)
            {
                throw new ConflictException(
                    $"ویژگی با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.Feature,
                    nameof(Feature.Name));
            }

            feature.UpdateFeatureName(dto.Name, _currentUserService.UserId);
            feature.UpdateFeatureMultiplicity(dto.Multiplicity, _currentUserService.UserId);

            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    feature.Activate(_currentUserService.UserId);
                else if (status == EntityStatus.Inactive)
                    feature.Inactivate(_currentUserService.UserId);
            }

            _featureRepository.Update(feature);
            await _featureRepository.SaveChangesAsync();

            return feature.ToDto();
        }

        /// <summary>
        /// حذف منطقی ویژگی
        /// </summary>
        /// <param name="id">شناسه ویژگی برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var feature = await _featureRepository.GetByIdAsync(id);
            if (feature is null)
            {
                throw new NotFoundException(
                    $"ویژگی با شناسه {id} یافت نشد.",
                    DomainConcept.Feature,
                    nameof(Feature.Id));
            }

            feature.Delete(_currentUserService.UserId);

            _featureRepository.Update(feature);
            await _featureRepository.SaveChangesAsync();
        }
    }
}