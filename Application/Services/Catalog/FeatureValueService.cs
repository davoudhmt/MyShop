using Application.DTOs.Catalog.FeatureValue;
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
    /// پیاده‌سازی سرویس مدیریت مقادیر ویژگی (Feature Values)
    /// </summary>
    public class FeatureValueService : IFeatureValueService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IFeatureValueRepository _featureValueRepository;
        private readonly IFeatureRepository _featureRepository;

        /// <summary>
        /// سازنده سرویس مقدار ویژگی
        /// </summary>
        /// <param name="featureValueRepository">ریپازیتوری مقدار ویژگی</param>
        /// <param name="featureRepository">ریپازیتوری ویژگی (برای اعتبارسنجی)</param>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        public FeatureValueService(
            IFeatureValueRepository featureValueRepository,
            IFeatureRepository featureRepository,
            ICurrentUserService currentUserService)
        {
            _featureValueRepository = featureValueRepository;
            _featureRepository = featureRepository;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// دریافت همه مقادیر ویژگی
        /// </summary>
        /// <returns>لیست تمام مقادیر ویژگی به صورت DTO</returns>
        public async Task<List<FeatureValueDto>> GetAllAsync()
        {
            var featureValues = await _featureValueRepository.GetAllAsync();
            return await featureValues.ToDtoListAsync(_featureRepository);
        }

        /// <summary>
        /// دریافت مقدار ویژگی با شناسه
        /// </summary>
        /// <param name="id">شناسه مقدار ویژگی</param>
        /// <returns>اطلاعات مقدار ویژگی یا null در صورت عدم وجود</returns>
        public async Task<FeatureValueDto?> GetByIdAsync(long id)
        {
            var featureValue = await _featureValueRepository.GetByIdAsync(id);
            return featureValue is null ? null : await featureValue.ToDtoAsync(_featureRepository);
        }

        /// <summary>
        /// دریافت مقادیر یک ویژگی خاص
        /// </summary>
        /// <param name="featureId">شناسه ویژگی والد</param>
        /// <returns>لیست مقادیر ویژگی مرتبط به صورت DTO</returns>
        public async Task<List<FeatureValueDto>> GetByFeatureIdAsync(long featureId)
        {
            var featureValues = await _featureValueRepository.GetByFeatureIdAsync(featureId);
            return await featureValues.ToDtoListAsync(_featureRepository);
        }

        /// <summary>
        /// دریافت مقادیر ویژگی با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<FeatureValueDto>> GetPagedAsync(FeatureValueFilterDto filter)
        {
            var result = await _featureValueRepository.GetPagedAsync(filter);

            return new PagedResult<FeatureValueDto>
            {
                Items = await result.Items.ToDtoListAsync(_featureRepository),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// ساخت مقدار ویژگی جدید
        /// </summary>
        /// <param name="dto">اطلاعات مقدار ویژگی جدید</param>
        /// <returns>مقدار ویژگی ساخته شده به صورت DTO</returns>
        public async Task<FeatureValueDto> CreateAsync(CreateFeatureValueDto dto)
        {
            var feature = await _featureRepository.GetByIdAsync(dto.FeatureId);
            if (feature is null)
            {
                throw new NotFoundException(
                    $"ویژگی با شناسه {dto.FeatureId} یافت نشد.",
                    DomainConcept.Feature,
                    nameof(FeatureValue.FeatureId));
            }

            if (await _featureValueRepository.ExistsAsync(dto.FeatureId, dto.Value.Trim()))
            {
                throw new ConflictException(
                    $"مقدار '{dto.Value}' برای ویژگی '{feature.Name}' قبلاً ثبت شده است.",
                    DomainConcept.FeatureValue,
                    nameof(FeatureValue.Value));
            }

            var featureValue = new FeatureValue(
                createBy: _currentUserService.UserId,
                value: dto.Value,
                featureId: dto.FeatureId);

            await _featureValueRepository.AddAsync(featureValue);
            await _featureValueRepository.SaveChangesAsync();

            return await featureValue.ToDtoAsync(_featureRepository);
        }

        /// <summary>
        /// به‌روزرسانی مقدار ویژگی
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده</param>
        /// <returns>مقدار ویژگی به‌روزرسانی شده به صورت DTO</returns>
        public async Task<FeatureValueDto> UpdateAsync(UpdateFeatureValueDto dto)
        {
            var featureValue = await _featureValueRepository.GetByIdAsync(dto.Id);
            if (featureValue is null)
            {
                throw new NotFoundException(
                    $"مقدار ویژگی با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.FeatureValue,
                    nameof(FeatureValue.Id));
            }

            var exists = await _featureValueRepository.ExistsAsync(featureValue.FeatureId, dto.Value.Trim());
            if (exists)
            {
                var existingByValue = await _featureValueRepository.GetByFeatureIdAsync(featureValue.FeatureId);
                var isSameRecord = existingByValue.Any(fv => fv.Id == dto.Id && fv.Value == dto.Value.Trim());

                if (!isSameRecord)
                {
                    throw new ConflictException(
                        $"مقدار '{dto.Value}' برای این ویژگی قبلاً ثبت شده است.",
                        DomainConcept.FeatureValue,
                        nameof(FeatureValue.Value));
                }
            }

            featureValue.UpdateValue(dto.Value, _currentUserService.UserId);

            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    featureValue.Activate(_currentUserService.UserId);
                else if (status == EntityStatus.Inactive)
                    featureValue.Inactivate(_currentUserService.UserId);
            }

            _featureValueRepository.Update(featureValue);
            await _featureValueRepository.SaveChangesAsync();

            return await featureValue.ToDtoAsync(_featureRepository);
        }

        /// <summary>
        /// حذف منطقی مقدار ویژگی
        /// </summary>
        /// <param name="id">شناسه مقدار ویژگی برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var featureValue = await _featureValueRepository.GetByIdAsync(id);
            if (featureValue is null)
            {
                throw new NotFoundException(
                    $"مقدار ویژگی با شناسه {id} یافت نشد.",
                    DomainConcept.FeatureValue,
                    nameof(FeatureValue.Id));
            }

            featureValue.Delete(_currentUserService.UserId);

            _featureValueRepository.Update(featureValue);
            await _featureValueRepository.SaveChangesAsync();
        }
    }
}