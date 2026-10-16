using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Catalog.ProductImage;
using Application.DTOs.Catalog.Variant;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده برای تبدیل موجودیت Variant به DTO
/// </summary>
public static class VariantMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت Variant به شیء VariantDto
    /// (با حفظ Projection همزمان برای جلوگیری از کوئری‌های اضافی N+1)
    /// </summary>
    public static VariantDto ToDto(this Variant variant)
    {
        return new VariantDto
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            Price = variant.Price,
            DiscountPercent = variant.DiscountPercent,
            Stock = variant.Stock,
            Sku = variant.SKU.Value,
            FeatureValues = variant.FeatureValues.Select(fv => new FeatureValueDto
            {
                Id = fv.Id,
                Value = fv.Value,
                FeatureId = fv.FeatureId
            }).ToList(),
            Images = variant.ProductImages.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl.Value ?? string.Empty,
                IsMain = i.IsMain,
                AltText = i.AltText
            }).ToList()
        };
    }

    /// <summary>
    /// تبدیل لیستی از Variant به لیست VariantDto
    /// </summary>
    public static List<VariantDto> ToDtoList(this IEnumerable<Variant> variants)
    {
        return variants.Select(ToDto).ToList();
    }
}