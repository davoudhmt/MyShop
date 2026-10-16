using Application.DTOs.Catalog.ProductType;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت ProductType به DTO و بالعکس
/// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود
/// </summary>
public static class ProductTypeMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت ProductType به شیء ProductTypeDto
    /// </summary>
    /// <param name="productType">موجودیت نوع محصول</param>
    /// <returns>شیء DTO نوع محصول</returns>
    public static ProductTypeDto ToDto(this ProductType productType)
    {
        return new ProductTypeDto
        {
            Id = productType.Id,
            Name = productType.Name,
            Description = productType.Description,
            Status = productType.Status.ToString(),
            CreateDate = productType.CreateDate,
            UpdateDate = productType.UpdateDate
        };
    }

    /// <summary>
    /// تبدیل لیستی از موجودیت‌های ProductType به لیست ProductTypeDto
    /// </summary>
    /// <param name="productTypes">لیست موجودیت‌های نوع محصول</param>
    /// <returns>لیست DTOهای نوع محصول</returns>
    public static List<ProductTypeDto> ToDtoList(this IEnumerable<ProductType> productTypes)
    {
        return productTypes.Select(ToDto).ToList();
    }
}