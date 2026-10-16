using Application.DTOs.Catalog.Category;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت Category به DTO و بالعکس
/// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود
/// </summary>
public static class CategoryMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت Category به شیء CategoryDto
    /// </summary>
    /// <param name="category">موجودیت دسته‌بندی</param>
    /// <returns>شیء DTO دسته‌بندی</returns>
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl?.Value,
            CategoryCode = category.CategoryCode.Value,
            ParentCategoryId = category.ParentCategoryId,
            Status = category.Status.ToString(),
            CreateDate = category.CreateDate,
            UpdateDate = category.UpdateDate
        };
    }

    /// <summary>
    /// تبدیل لیستی از موجودیت‌های Category به لیست CategoryDto
    /// </summary>
    /// <param name="categories">لیست موجودیت‌های دسته‌بندی</param>
    /// <returns>لیست DTOهای دسته‌بندی</returns>
    public static List<CategoryDto> ToDtoList(this IEnumerable<Category> categories)
    {
        return categories.Select(ToDto).ToList();
    }
}