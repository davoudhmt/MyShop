using Application.DTOs.Catalog.ProductImage;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت ProductImage به DTO و بالعکس
/// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود
/// </summary>
public static class ProductImageMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت ProductImage به شیء ProductImageDto
    /// </summary>
    /// <param name="image">موجودیت تصویر محصول</param>
    /// <returns>شیء DTO تصویر محصول</returns>
    public static ProductImageDto ToDto(this ProductImage image)
    {
        return new ProductImageDto
        {
            Id = image.Id,
            ImageUrl = image.ImageUrl.Value ?? string.Empty,
            ProductId = image.ProductId,
            VariantId = image.VariantId,
            IsMain = image.IsMain,
            AltText = image.AltText,
            Status = image.Status.ToString(),
            CreateDate = image.CreateDate,
            UpdateDate = image.UpdateDate
        };
    }

    /// <summary>
    /// تبدیل لیستی از موجودیت‌های ProductImage به لیست ProductImageDto
    /// </summary>
    /// <param name="images">لیست موجودیت‌های تصویر محصول</param>
    /// <returns>لیست DTOهای تصویر محصول</returns>
    public static List<ProductImageDto> ToDtoList(this IEnumerable<ProductImage> images)
    {
        return images.Select(ToDto).ToList();
    }
}