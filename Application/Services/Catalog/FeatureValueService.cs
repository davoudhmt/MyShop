using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Common;
using Application.Interfaces.Catalog;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس مقدار ویژگی
    /// </summary>
    public class FeatureValueService : IFeatureValueService
    {
        /// <summary>
        /// شناسه کاربر سیستم (موقتاً ثابت)
        /// </summary>
        private const long SystemUser = 1;

        private readonly IFeatureValueRepository _featureValueRepository;
        private readonly IFeatureRepository _featureRepository;

        /// <summary>
        /// سازنده سرویس مقدار ویژگی
        /// </summary>
        public FeatureValueService(
            IFeatureValueRepository featureValueRepository,
            IFeatureRepository featureRepository)
        {
            _featureValueRepository = featureValueRepository;
            _featureRepository = featureRepository;
        }

        /// <summary>
        /// دریافت همه مقادیر ویژگی
        /// </summary>
        public async Task<List<FeatureValueDto>> GetAllAsync()
        {
            var featureValues = await _featureValueRepository.GetAllAsync();

            var result = new List<FeatureValueDto>();
            foreach (var fv in featureValues)
            {
                result.Add(await ToDtoAsync(fv));
            }
            return result;
        }

        /// <summary>
        /// دریافت مقدار ویژگی با شناسه
        /// </summary>
        public async Task<FeatureValueDto?> GetByIdAsync(long id)
        {
            var featureValue = await _featureValueRepository.GetByIdAsync(id);
            return featureValue is null ? null : await ToDtoAsync(featureValue);
        }

        /// <summary>
        /// دریافت مقادیر یک ویژگی خاص
        /// </summary>
        public async Task<List<FeatureValueDto>> GetByFeatureIdAsync(long featureId)
        {
            var featureValues = await _featureValueRepository.GetByFeatureIdAsync(featureId);

            var result = new List<FeatureValueDto>();
            foreach (var fv in featureValues)
            {
                result.Add(await ToDtoAsync(fv));
            }
            return result;
        }

        /// <summary>
        /// دریافت مقادیر ویژگی با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<FeatureValueDto>> GetPagedAsync(FeatureValueFilterDto filter)
        {
            var result = await _featureValueRepository.GetPagedAsync(filter);

            var items = new List<FeatureValueDto>();
            foreach (var fv in result.Items)
            {
                items.Add(await ToDtoAsync(fv));
            }

            return new PagedResult<FeatureValueDto>
            {
                Items = items,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// ساخت مقدار ویژگی جدید
        /// </summary>
        public async Task<FeatureValueDto> CreateAsync(CreateFeatureValueDto dto)
        {
            // ===== بررسی وجود ویژگی =====
            var feature = await _featureRepository.GetByIdAsync(dto.FeatureId);
            if (feature is null)
                throw new InvalidOperationException($"ویژگی با شناسه {dto.FeatureId} یافت نشد.");

            // ===== بررسی تکراری نبودن مقدار =====
            if (await _featureValueRepository.ExistsAsync(dto.FeatureId, dto.Value.Trim()))
                throw new InvalidOperationException($"مقدار '{dto.Value}' برای ویژگی '{feature.Name}' قبلاً ثبت شده است.");

            // ===== ساخت موجودیت =====
            var featureValue = new FeatureValue(
                createBy: SystemUser,
                value: dto.Value,
                featureId: dto.FeatureId);

            await _featureValueRepository.AddAsync(featureValue);
            await _featureValueRepository.SaveChangesAsync();

            return await ToDtoAsync(featureValue);
        }

        /// <summary>
        /// به‌روزرسانی مقدار ویژگی
        /// </summary>
        public async Task<FeatureValueDto> UpdateAsync(UpdateFeatureValueDto dto)
        {
            var featureValue = await _featureValueRepository.GetByIdAsync(dto.Id);
            if (featureValue is null)
                throw new InvalidOperationException($"مقدار ویژگی با شناسه {dto.Id} یافت نشد.");

            // ===== بررسی تکراری نبودن مقدار (به جز خودش) =====
            var exists = await _featureValueRepository.ExistsAsync(featureValue.FeatureId, dto.Value.Trim());
            if (exists)
            {
                // باید چک کنیم که آیا این مقدار تکراری خود همین رکورد هست یا نه
                var existingByValue = await _featureValueRepository.GetByFeatureIdAsync(featureValue.FeatureId);
                var isSameRecord = existingByValue.Any(fv => fv.Id == dto.Id && fv.Value == dto.Value.Trim());
                if (!isSameRecord)
                    throw new InvalidOperationException($"مقدار '{dto.Value}' برای این ویژگی قبلاً ثبت شده است.");
            }

            // ===== به‌روزرسانی فیلدها =====
            featureValue.UpdateValue(dto.Value, SystemUser);

            // ===== تغییر وضعیت =====
            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    featureValue.Activate(SystemUser);
                else if (status == EntityStatus.Inactive)
                    featureValue.Inactivate(SystemUser);
            }

            _featureValueRepository.Update(featureValue);
            await _featureValueRepository.SaveChangesAsync();

            return await ToDtoAsync(featureValue);
        }

        /// <summary>
        /// حذف منطقی مقدار ویژگی
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            var featureValue = await _featureValueRepository.GetByIdAsync(id);
            if (featureValue is null)
                throw new InvalidOperationException($"مقدار ویژگی با شناسه {id} یافت نشد.");

            featureValue.Delete(SystemUser);

            _featureValueRepository.Update(featureValue);
            await _featureValueRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تبدیل موجودیت به DTO
        /// </summary>
        private async Task<FeatureValueDto> ToDtoAsync(FeatureValue featureValue)
        {
            // دریافت نام ویژگی
            var feature = await _featureRepository.GetByIdAsync(featureValue.FeatureId);

            return new FeatureValueDto
            {
                Id = featureValue.Id,
                Value = featureValue.Value,
                FeatureId = featureValue.FeatureId,
                FeatureName = feature?.Name ?? string.Empty,
                Status = featureValue.Status.ToString(),
                CreateDate = featureValue.CreateDate,
                UpdateDate = featureValue.UpdateDate
            };
        }
    }
}