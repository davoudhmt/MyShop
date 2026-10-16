using Application.DTOs.Catalog.FeatureValue;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت FeatureValue به DTO و بالعکس
/// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود
/// </summary>
public static class FeatureValueMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت FeatureValue به شیء FeatureValueDto
    /// </summary>
    /// <param name="featureValue">موجودیت مقدار ویژگی</param>
    /// <param name="featureRepository">ریپازیتوری ویژگی برای دریافت نام ویژگی</param>
    /// <returns>شیء DTO مقدار ویژگی</returns>
    public static async Task<FeatureValueDto> ToDtoAsync(this FeatureValue featureValue, IFeatureRepository featureRepository)
    {
        var feature = await featureRepository.GetByIdAsync(featureValue.FeatureId);

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

    /// <summary>
    /// تبدیل لیستی از موجودیت‌های FeatureValue به لیست FeatureValueDto
    /// </summary>
    /// <param name="featureValues">لیست موجودیت‌های مقدار ویژگی</param>
    /// <param name="featureRepository">ریپازیتوری ویژگی برای دریافت نام ویژگی‌ها</param>
    /// <returns>لیست DTOهای مقدار ویژگی</returns>
    public static async Task<List<FeatureValueDto>> ToDtoListAsync(this IEnumerable<FeatureValue> featureValues, IFeatureRepository featureRepository)
    {
        var result = new List<FeatureValueDto>();

        foreach (var fv in featureValues)
        {
            result.Add(await ToDtoAsync(fv, featureRepository));
        }

        return result;
    }
}