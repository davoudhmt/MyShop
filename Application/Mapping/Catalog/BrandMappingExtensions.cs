using Application.DTOs.Catalog.Brand;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت Brand به DTO و بالعکس
/// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود
/// </summary>
public static class BrandMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت Brand به شیء BrandDto
    /// </summary>
    /// <param name="brand">موجودیت برند</param>
    /// <returns>شیء DTO برند</returns>
    public static BrandDto ToDto(this Brand brand)
    {
        return new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description,
            ImageUrl = brand.ImageUrl,
            Status = brand.Status.ToString(),
            CreateDate = brand.CreateDate,
            UpdateDate = brand.UpdateDate
        };
    }

    /// <summary>
    /// تبدیل لیستی از موجودیت‌های Brand به لیست BrandDto
    /// </summary>
    /// <param name="brands">لیست موجودیت‌های برند</param>
    /// <returns>لیست DTOهای برند</returns>
    public static List<BrandDto> ToDtoList(this IEnumerable<Brand> brands)
    {
        return brands.Select(ToDto).ToList();
    }
}