using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Catalog.Product;
using Application.DTOs.Catalog.ProductImage;
using Application.DTOs.Catalog.Seo;
using Application.DTOs.Catalog.Tag;
using Domain.Entities.Catalog;

namespace Application.Mappings.Catalog;

/// <summary>
/// متدهای توسعه‌دهنده برای تبدیل موجودیت Product به DTO
/// </summary>
public static class ProductMappingExtensions
{
    /// <summary>
    /// تبدیل موجودیت Product به شیء ProductDto
    /// </summary>
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            ShortDescription = product.ShortDescription,
            Description = product.Description,
            ProductCode = product.ProductCode.Value,
            Slug = product.Slug.Value,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            ProductTypeId = product.ProductTypeId,
            Status = product.Status.ToString(),
            CreateDate = product.CreateDate,
            UpdateDate = product.UpdateDate,

            // استفاده از Projection ساده برای حفظ پرفورمنس (همان منطق کد اصلی شما)
            Tags = product.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList(),

            FeatureValues = product.FeatureValues.Select(fv => new FeatureValueDto
            {
                Id = fv.Id,
                Value = fv.Value,
                FeatureId = fv.FeatureId
            }).ToList(),

            Images = product.ProductImages.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl.Value ?? string.Empty,
                IsMain = i.IsMain,
                AltText = i.AltText
            }).ToList(),

            Variants = product.Variants.ToDtoList(),

            Seo = product.Seo != null ? new SeoDto
            {
                Title = product.Seo.Title,
                H1 = product.Seo.H1,
                Description = product.Seo.Description,
                MetaDescription = product.Seo.MetaDescription,
                MetaKeywords = product.Seo.MetaKeywords
            } : null
        };
    }

    /// <summary>
    /// تبدیل لیستی از Product به لیست ProductDto
    /// </summary>
    public static List<ProductDto> ToDtoList(this IEnumerable<Product> products)
    {
        return products.Select(ToDto).ToList();
    }
}