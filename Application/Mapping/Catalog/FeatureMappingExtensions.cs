using Application.DTOs.Catalog.Feature;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت Feature به DTO و بالعکس
/// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود
/// </summary>
public static class FeatureMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت Feature به شیء FeatureDto
    /// </summary>
    /// <param name="feature">موجودیت ویژگی</param>
    /// <returns>شیء DTO ویژگی</returns>
    public static FeatureDto ToDto(this Feature feature)
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

    /// <summary>
    /// تبدیل لیستی از موجودیت‌های Feature به لیست FeatureDto
    /// </summary>
    /// <param name="features">لیست موجودیت‌های ویژگی</param>
    /// <returns>لیست DTOهای ویژگی</returns>
    public static List<FeatureDto> ToDtoList(this IEnumerable<Feature> features)
    {
        return features.Select(ToDto).ToList();
    }
}