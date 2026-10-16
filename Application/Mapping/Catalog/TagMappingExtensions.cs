using Application.DTOs.Catalog.Tag;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده (Extension) برای تبدیل موجودیت Tag به DTO و بالعکس
/// این کلاس منطق مپینگ را از سرویس جدا می‌کند تا اصل Single Responsibility رعایت شود
/// </summary>
public static class TagMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت Tag به شیء TagDto
    /// </summary>
    /// <param name="tag">موجودیت برچسب</param>
    /// <returns>شیء DTO برچسب</returns>
    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Status = tag.Status.ToString(),
            CreateDate = tag.CreateDate,
            UpdateDate = tag.UpdateDate
        };
    }

    /// <summary>
    /// تبدیل لیستی از موجودیت‌های Tag به لیست TagDto
    /// </summary>
    /// <param name="tags">لیست موجودیت‌های برچسب</param>
    /// <returns>لیست DTOهای برچسب</returns>
    public static List<TagDto> ToDtoList(this IEnumerable<Tag> tags)
    {
        return tags.Select(ToDto).ToList();
    }
}