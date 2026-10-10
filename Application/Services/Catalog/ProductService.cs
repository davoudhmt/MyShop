using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Catalog.Product;
using Application.DTOs.Catalog.ProductImage;
using Application.DTOs.Catalog.Seo;
using Application.DTOs.Catalog.Tag;
using Application.DTOs.Catalog.Variant;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services;
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
        private const long SystemUser = 1; // TODO: جایگزینی با ICurrentUserService

        private readonly IProductRepository _productRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFeatureValueRepository _featureValueRepository;

        public ProductService(
            IProductRepository productRepository,
            ITagRepository tagRepository,
            IFileStorageService fileStorageService,
            IFeatureValueRepository featureValueRepository)
        {
            _productRepository = productRepository;
            _tagRepository = tagRepository;
            _fileStorageService = fileStorageService;
            _featureValueRepository = featureValueRepository;
        }

        public async Task<ProductDto?> GetByIdAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product is null ? null : MapToDto(product);
        }

        public async Task<ProductDto?> GetBySlugAsync(string slug)
        {
            var product = await _productRepository.GetBySlugAsync(slug);
            return product is null ? null : MapToDto(product);
        }

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

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            if (await _productRepository.ExistsByCodeAsync(dto.ProductCode))
                throw new DomainException("کد محصول تکراری است.", DomainConcept.Product, nameof(dto.ProductCode));

            if (await _productRepository.ExistsBySlugAsync(dto.Slug))
                throw new DomainException("اسلاگ محصول تکراری است.", DomainConcept.Product, nameof(dto.Slug));

            var productCode = new ProductCode(dto.ProductCode);
            var slug = new Slug(dto.Slug);

            Seo? seo = null;
            if (dto.Seo != null)
            {
                seo = new Seo(dto.Seo.Title, dto.Seo.H1, dto.Seo.Description, dto.Seo.MetaDescription, dto.Seo.MetaKeywords);
            }

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

            if (dto.TagIds.Any())
            {
                var tags = await _tagRepository.GetByIdsAsync(dto.TagIds);
                foreach (var tag in tags)
                {
                    product.AddTag(tag);
                }
            }

            // ✅ ۱. ذخیره اولیه محصول برای دریافت Id معتبر (رفع باگ DomainException)
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            // ✅ ۲. پردازش واریانت‌ها (حالا product.Id > 0 است و معتبر می‌باشد)
            if (dto.Variants != null && dto.Variants.Any())
            {
                var allFeatureValueIds = dto.Variants.SelectMany(v => v.FeatureValueIds).Distinct().ToList();
                var featureValuesDict = new Dictionary<long, FeatureValue>();

                if (allFeatureValueIds.Any())
                {
                    var fetchedFeatureValues = await _featureValueRepository.GetByIdsAsync(allFeatureValueIds);
                    featureValuesDict = fetchedFeatureValues.ToDictionary(fv => fv.Id);
                }

                foreach (var variantDto in dto.Variants)
                {
                    var sku = new SKU(variantDto.Sku);

                    var variant = new Variant(
                        createBy: SystemUser,
                        productId: product.Id, // ✅ اکنون Id معتبر است
                        price: variantDto.Price,
                        stock: variantDto.Stock,
                        sku: sku,
                        discountPercent: variantDto.DiscountPercent
                    );

                    foreach (var fvId in variantDto.FeatureValueIds)
                    {
                        if (featureValuesDict.TryGetValue(fvId, out var featureValue))
                        {
                            variant.AddFeatureValue(featureValue);
                        }
                    }

                    foreach (var imgUrl in variantDto.ImageUrls.Where(url => !string.IsNullOrWhiteSpace(url)))
                    {
                        var image = new ProductImage(
                            createBy: SystemUser,
                            imageUrl: new ImageUrl(imgUrl),
                            productId: product.Id,
                            variantId: null,
                            isMain: false,
                            altText: null
                        );
                        variant.AddImage(image);
                    }
                }

                // ✅ ۳. ذخیره نهایی واریانت‌ها
                await _productRepository.SaveChangesAsync();
            }

            return MapToDto(product);
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product is null)
                throw new InvalidOperationException($"محصول با شناسه {dto.Id} یافت نشد.");

            if (product.ProductCode.Value != dto.ProductCode && await _productRepository.ExistsByCodeAsync(dto.ProductCode))
                throw new DomainException("کد محصول تکراری است.", DomainConcept.Product, nameof(dto.ProductCode));

            if (product.Slug.Value != dto.Slug && await _productRepository.ExistsBySlugAsync(dto.Slug))
                throw new DomainException("اسلاگ محصول تکراری است.", DomainConcept.Product, nameof(dto.Slug));

            product.UpdateName(dto.Name, SystemUser);
            product.UpdateShortDescription(dto.ShortDescription, SystemUser);
            product.UpdateDescription(dto.Description, SystemUser);
            product.UpdateCategory(dto.CategoryId, SystemUser);
            product.UpdateProductType(dto.ProductTypeId, SystemUser);
            product.UpdateBrand(dto.BrandId, SystemUser);
            product.UpdateSlug(new Slug(dto.Slug), SystemUser);

            if (dto.Seo != null)
            {
                var newSeo = new Seo(dto.Seo.Title, dto.Seo.H1, dto.Seo.Description, dto.Seo.MetaDescription, dto.Seo.MetaKeywords);
                product.UpdateSeo(newSeo, SystemUser);
            }

            if (dto.TagIds.Any())
            {
                var tags = await _tagRepository.GetByIdsAsync(dto.TagIds);
                foreach (var tag in tags)
                {
                    product.AddTag(tag);
                }
            }


            // ✅ مدیریت هوشمند واریانت‌ها در به‌روزرسانی
            if (dto.Variants != null)
            {
                var incomingVariantIds = dto.Variants.Where(v => v.Id > 0).Select(v => v.Id).ToList();

                // الف) حذف واریانت‌هایی که در درخواست جدید نیستند
                var variantsToDelete = product.Variants.Where(v => !incomingVariantIds.Contains(v.Id)).ToList();
                foreach (var variantToDelete in variantsToDelete)
                {
                    // ۱. حذف فیزیکی تصاویر واریانت حذف‌شده
                    var imageUrls = variantToDelete.ProductImages
                        .Select(img => img.ImageUrl?.Value)
                        .Where(url => !string.IsNullOrEmpty(url))
                        .OfType<string>()
                        .ToList();

                    if (imageUrls.Any())
                    {
                        await _fileStorageService.DeleteMultipleAsync(imageUrls);
                    }

                    // ۲. حذف از Aggregate Root
                    product.RemoveVariant(variantToDelete);
                }

                // ب) پردازش واریانت‌های جدید و ویرایش‌شده
                var allFeatureValueIds = dto.Variants.SelectMany(v => v.FeatureValueIds).Distinct().ToList();
                var featureValuesDict = new Dictionary<long, FeatureValue>();
                if (allFeatureValueIds.Any())
                {
                    var fetched = await _featureValueRepository.GetByIdsAsync(allFeatureValueIds);
                    featureValuesDict = fetched.ToDictionary(fv => fv.Id);
                }

                foreach (var variantDto in dto.Variants)
                {
                    if (variantDto.Id > 0)
                    {
                        // ویرایش واریانت موجود
                        var existingVariant = product.Variants.FirstOrDefault(v => v.Id == variantDto.Id);
                        if (existingVariant != null)
                        {
                            existingVariant.UpdatePrice(variantDto.Price, SystemUser);
                            existingVariant.UpdateStock(variantDto.Stock, SystemUser);
                            existingVariant.UpdateDiscountPercent(variantDto.DiscountPercent, SystemUser);
                            // نکته: برای تغییر SKU یا ویژگی‌ها در DDD معمولاً متدهای اختصاصی نیاز است.
                            // در اینجا برای سادگی، فقط فیلدهای پایه آپدیت شدند.
                        }
                    }
                    else
                    {
                        // ساخت واریانت جدید
                        var sku = new SKU(variantDto.Sku);
                        var newVariant = new Variant(
                            createBy: SystemUser,
                            productId: product.Id,
                            price: variantDto.Price,
                            stock: variantDto.Stock,
                            sku: sku,
                            discountPercent: variantDto.DiscountPercent
                        );

                        foreach (var fvId in variantDto.FeatureValueIds)
                        {
                            if (featureValuesDict.TryGetValue(fvId, out var fv))
                                newVariant.AddFeatureValue(fv);
                        }

                        foreach (var imgUrl in variantDto.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                        {
                            var newImage = new ProductImage(
                                createBy: SystemUser,
                                imageUrl: new ImageUrl(imgUrl),
                                productId: product.Id,
                                variantId: null,
                                isMain: false,
                                altText: null
                            );
                            newVariant.AddImage(newImage);
                        }

                        product.AddVariant(newVariant);
                    }
                }
            }

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task DeleteAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                throw new InvalidOperationException($"محصول با شناسه {id} یافت نشد.");

            var mainImageUrls = product.ProductImages
                .Select(img => img.ImageUrl?.Value)
                .Where(url => !string.IsNullOrEmpty(url))
                .OfType<string>()
                .ToList();

            var variantImageUrls = product.Variants
                .SelectMany(variant => variant.ProductImages)
                .Select(img => img.ImageUrl?.Value)
                .Where(url => !string.IsNullOrEmpty(url))
                .OfType<string>()
                .ToList();

            var allImageUrls = mainImageUrls.Concat(variantImageUrls).Distinct().ToList();

            if (allImageUrls.Any())
            {
                await _fileStorageService.DeleteMultipleAsync(allImageUrls);
            }

            product.Delete(SystemUser);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task SetMainImageAsync(long productId, long imageId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
                throw new InvalidOperationException($"محصول با شناسه {productId} یافت نشد.");

            product.SetMainImage(imageId, SystemUser);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// حذف یک واریانت خاص از محصول و حذف فیزیکی تصاویر آن
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <param name="variantId">شناسه واریانت</param>
        public async Task RemoveVariantAsync(long productId, long variantId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
                throw new InvalidOperationException($"محصول با شناسه {productId} یافت نشد.");

            var variant = product.Variants.FirstOrDefault(v => v.Id == variantId);
            if (variant is null)
                throw new InvalidOperationException($"واریانت با شناسه {variantId} در این محصول یافت نشد.");

            // ۱. حذف فیزیکی تصاویر این واریانت
            var imageUrls = variant.ProductImages
                .Select(img => img.ImageUrl?.Value)
                .Where(url => !string.IsNullOrEmpty(url))
                .OfType<string>()
                .ToList();

            if (imageUrls.Any())
            {
                await _fileStorageService.DeleteMultipleAsync(imageUrls);
            }

            // ۲. حذف واریانت از محصول
            product.RemoveVariant(variant);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

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
                Variants = product.Variants.Select(v => new VariantDto
                {
                    Id = v.Id,
                    ProductId = v.ProductId,
                    Price = v.Price,
                    DiscountPercent = v.DiscountPercent,
                    Stock = v.Stock,
                    Sku = v.SKU.Value,
                    FeatureValues = v.FeatureValues.Select(fv => new FeatureValueDto
                    {
                        Id = fv.Id,
                        Value = fv.Value,
                        FeatureId = fv.FeatureId
                    }).ToList(),
                    Images = v.ProductImages.Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl.Value ?? string.Empty,
                        IsMain = i.IsMain,
                        AltText = i.AltText
                    }).ToList()
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