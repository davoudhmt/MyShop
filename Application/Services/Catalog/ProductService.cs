using Application.DTOs.Catalog.Product;
using Application.DTOs.Catalog.ProductImage;
using Application.DTOs.Catalog.Seo;
using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت محصول
    /// </summary>
    public class ProductService : IProductService
    {
        private const long SystemUser = 1;

        private readonly IProductRepository _productRepository;
        private readonly ITagRepository _tagRepository;

        /// <summary>
        /// سازنده سرویس محصول
        /// </summary>
        public ProductService(IProductRepository productRepository, ITagRepository tagRepository)
        {
            _productRepository = productRepository;
            _tagRepository = tagRepository;
        }

        /// <summary>
        /// دریافت محصول با شناسه
        /// </summary>
        public async Task<ProductDto?> GetByIdAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product is null ? null : MapToDto(product);
        }

        /// <summary>
        /// دریافت محصول با اسلاگ
        /// </summary>
        public async Task<ProductDto?> GetBySlugAsync(string slug)
        {
            var product = await _productRepository.GetBySlugAsync(slug);
            return product is null ? null : MapToDto(product);
        }

        /// <summary>
        /// دریافت لیست محصولات با فیلتر و صفحه‌بندی
        /// </summary>
        public async Task<PagedResult<ProductDto>> GetPagedAsync(ProductFilterDto filter)
        {
            var pagedProducts = await _productRepository.GetPagedAsync(filter);

            return new PagedResult<ProductDto>
            {
                Items = pagedProducts.Items.Select(MapToDto).ToList(),
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize,
                TotalCount = pagedProducts.TotalCount
            };
        }

        /// <summary>
        /// ایجاد محصول جدید
        /// </summary>
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // ۱. بررسی یکتایی کد محصول و اسلاگ
            if (await _productRepository.ExistsByCodeAsync(dto.ProductCode))
                throw new DomainException("کد محصول تکراری است.", DomainConcept.Product, nameof(dto.ProductCode));

            if (await _productRepository.ExistsBySlugAsync(dto.Slug))
                throw new DomainException("اسلاگ محصول تکراری است.", DomainConcept.Product, nameof(dto.Slug));

            // ۲. ساخت Value Objectها
            var productCode = new ProductCode(dto.ProductCode);
            var slug = new Slug(dto.Slug);

            // ۳. مپ کردن Seo
            Seo? seo = null;
            if (dto.Seo != null)
            {
                seo = new Seo(dto.Seo.Title, dto.Seo.H1, dto.Seo.Description, dto.Seo.MetaDescription, dto.Seo.MetaKeywords);
            }

            // ۴. ساخت موجودیت Product
            var product = new Product(
                createBy: SystemUser,
                name: dto.Name,
                shortDescription: dto.ShortDescription,
                description: dto.Description,
                categoryId: dto.CategoryId,
                productTypeId: dto.ProductTypeId,
                productCode: productCode,
                slug: slug,
                seo: seo,
                brandId: dto.BrandId);

            // ۵. افزودن تگ‌ها
            if (dto.TagIds.Any())
            {
                var tags = await _tagRepository.GetByIdsAsync(dto.TagIds);
                foreach (var tag in tags)
                {
                    product.AddTag(tag);
                }
            }

            // ۶. ذخیره
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات محصول
        /// </summary>
        public async Task<ProductDto> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product is null)
                throw new InvalidOperationException($"محصول با شناسه {dto.Id} یافت نشد.");

            // ۱. بررسی یکتایی
            if (product.ProductCode.Value != dto.ProductCode && await _productRepository.ExistsByCodeAsync(dto.ProductCode))
                throw new DomainException("کد محصول تکراری است.", DomainConcept.Product, nameof(dto.ProductCode));

            if (product.Slug.Value != dto.Slug && await _productRepository.ExistsBySlugAsync(dto.Slug))
                throw new DomainException("اسلاگ محصول تکراری است.", DomainConcept.Product, nameof(dto.Slug));

            // ۲. به‌روزرسانی فیلدهای پایه
            product.UpdateName(dto.Name, SystemUser);
            product.UpdateShortDescription(dto.ShortDescription, SystemUser);
            product.UpdateDescription(dto.Description, SystemUser);
            product.UpdateCategory(dto.CategoryId, SystemUser);
            product.UpdateProductType(dto.ProductTypeId, SystemUser);
            product.UpdateBrand(dto.BrandId, SystemUser);
            product.UpdateSlug(new Slug(dto.Slug), SystemUser);

            // ۳. به‌روزرسانی Seo
            if (dto.Seo != null)
            {
                var newSeo = new Seo(dto.Seo.Title, dto.Seo.H1, dto.Seo.Description, dto.Seo.MetaDescription, dto.Seo.MetaKeywords);
                product.UpdateSeo(newSeo, SystemUser);
            }

            // ۴. به‌روزرسانی تگ‌ها
            if (dto.TagIds.Any())
            {
                var tags = await _tagRepository.GetByIdsAsync(dto.TagIds);
                foreach (var tag in tags)
                {
                    product.AddTag(tag);
                }
            }

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        /// <summary>
        /// حذف منطقی محصول
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                throw new InvalidOperationException($"محصول با شناسه {id} یافت نشد.");

            product.Delete(SystemUser);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تنظیم یک تصویر به عنوان تصویر اصلی (کاور) محصول
        /// </summary>
        public async Task SetMainImageAsync(long productId, long imageId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
                throw new InvalidOperationException($"محصول با شناسه {productId} یافت نشد.");

            // ✅ جادوی DDD اینجاست: خودِ Product مدیریت می‌کنه که بقیه تصاویر Unmark بشن
            product.SetMainImage(imageId, SystemUser);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تبدیل موجودیت به DTO
        /// </summary>
        private static ProductDto MapToDto(Product product)
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
                Tags = product.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList(),
                Images = product.ProductImages.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl.Value ?? string.Empty,
                    IsMain = i.IsMain,
                    AltText = i.AltText
                }).ToList(),
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
    }
}