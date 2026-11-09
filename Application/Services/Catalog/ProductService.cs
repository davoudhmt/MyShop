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
using Application.Mappings.Catalog;
using Domain.Constants;
using Domain.Entities.Catalog;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت محصول (Product)
    /// این سرویس مسئولیت مدیریت چرخه حیات محصول، شامل ایجاد، به‌روزرسانی، حذف و مدیریت واریانت‌ها و تصاویر را بر عهده دارد.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IProductRepository _productRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFeatureValueRepository _featureValueRepository;

        /// <summary>
        /// سازنده سرویس محصول و مقداردهی وابستگی‌ها
        /// </summary>
        /// <param name="productRepository">ریپازیتوری محصول</param>
        /// <param name="tagRepository">ریپازیتوری برچسب</param>
        /// <param name="fileStorageService">سرویس مدیریت فایل برای حذف تصاویر</param>
        /// <param name="featureValueRepository">ریپازیتوری مقادیر ویژگی</param>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        public ProductService(
            IProductRepository productRepository,
            ITagRepository tagRepository,
            IFileStorageService fileStorageService,
            IFeatureValueRepository featureValueRepository,
            ICurrentUserService currentUserService)
        {
            _productRepository = productRepository;
            _tagRepository = tagRepository;
            _fileStorageService = fileStorageService;
            _featureValueRepository = featureValueRepository;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// دریافت محصول با شناسه
        /// </summary>
        /// <param name="id">شناسه محصول</param>
        /// <returns>اطلاعات محصول یا null در صورت عدم وجود</returns>
        public async Task<ProductDto?> GetByIdAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product?.ToDto();
        }

        /// <summary>
        /// دریافت محصول با استفاده از اسلاگ (Slug)
        /// </summary>
        /// <param name="slug">آدرس متنی یکتای محصول</param>
        /// <returns>اطلاعات محصول یا null در صورت عدم وجود</returns>
        public async Task<ProductDto?> GetBySlugAsync(string slug)
        {
            var product = await _productRepository.GetBySlugAsync(slug);
            return product?.ToDto();
        }

        /// <summary>
        /// دریافت لیست صفحه‌بندی شده محصولات بر اساس فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای جستجو و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<ProductDto>> GetPagedAsync(ProductFilterDto filter)
        {
            var pagedProducts = await _productRepository.GetPagedAsync(filter);

            return new PagedResult<ProductDto>
            {
                Items = pagedProducts.Items.ToDtoList(),
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize,
                TotalCount = pagedProducts.TotalCount
            };
        }

        /// <summary>
        /// ساخت محصول جدید به همراه واریانت‌ها و تصاویر مرتبط
        /// </summary>
        /// <param name="dto">اطلاعات محصول جدید</param>
        /// <returns>محصول ساخته شده به صورت DTO</returns>
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            if (await _productRepository.ExistsByCodeAsync(dto.ProductCode))
                throw new ConflictException("کد محصول تکراری است.", DomainConcept.Product, nameof(dto.ProductCode));

            if (await _productRepository.ExistsBySlugAsync(dto.Slug))
                throw new ConflictException("اسلاگ محصول تکراری است.", DomainConcept.Product, nameof(dto.Slug));

            var productCode = new ProductCode(dto.ProductCode);
            var slug = new Slug(dto.Slug);

            Seo? seo = null;
            if (dto.Seo != null)
            {
                seo = new Seo(dto.Seo.Title, dto.Seo.H1, dto.Seo.Description, dto.Seo.MetaDescription, dto.Seo.MetaKeywords);
            }

            var product = new Product(
                createBy: _currentUserService.UserId,
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

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

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
                        createBy: _currentUserService.UserId,
                        productId: product.Id,
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
                            createBy: _currentUserService.UserId,
                            imageUrl: new ImageUrl(imgUrl),
                            productId: product.Id,
                            variantId: null,
                            isMain: false,
                            altText: null
                        );
                        variant.AddImage(image);
                    }
                }

                await _productRepository.SaveChangesAsync();
            }

            return product.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات محصول و مدیریت هوشمند واریانت‌ها (افزودن، ویرایش یا حذف)
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده محصول</param>
        /// <returns>محصول به‌روزرسانی شده به صورت DTO</returns>
        public async Task<ProductDto> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product is null)
                throw new NotFoundException($"محصول با شناسه {dto.Id} یافت نشد.", DomainConcept.Product, nameof(Product.Id));

            if (product.ProductCode.Value != dto.ProductCode && await _productRepository.ExistsByCodeAsync(dto.ProductCode))
                throw new ConflictException("کد محصول تکراری است.", DomainConcept.Product, nameof(dto.ProductCode));

            if (product.Slug.Value != dto.Slug && await _productRepository.ExistsBySlugAsync(dto.Slug))
                throw new ConflictException("اسلاگ محصول تکراری است.", DomainConcept.Product, nameof(dto.Slug));

            product.UpdateName(dto.Name, _currentUserService.UserId);
            product.UpdateShortDescription(dto.ShortDescription, _currentUserService.UserId);
            product.UpdateDescription(dto.Description, _currentUserService.UserId);
            product.UpdateCategory(dto.CategoryId, _currentUserService.UserId);
            product.UpdateProductType(dto.ProductTypeId, _currentUserService.UserId);
            product.UpdateBrand(dto.BrandId, _currentUserService.UserId);
            product.UpdateSlug(new Slug(dto.Slug), _currentUserService.UserId);

            if (dto.Seo != null)
            {
                var newSeo = new Seo(dto.Seo.Title, dto.Seo.H1, dto.Seo.Description, dto.Seo.MetaDescription, dto.Seo.MetaKeywords);
                product.UpdateSeo(newSeo, _currentUserService.UserId);
            }

            if (dto.TagIds.Any())
            {
                var tags = await _tagRepository.GetByIdsAsync(dto.TagIds);
                foreach (var tag in tags)
                {
                    product.AddTag(tag);
                }
            }

            if (dto.Variants != null)
            {
                var incomingVariantIds = dto.Variants.Where(v => v.Id > 0).Select(v => v.Id).ToList();

                var variantsToDelete = product.Variants.Where(v => !incomingVariantIds.Contains(v.Id)).ToList();
                foreach (var variantToDelete in variantsToDelete)
                {
                    var imageUrls = variantToDelete.ProductImages
                        .Select(img => img.ImageUrl?.Value)
                        .Where(url => !string.IsNullOrEmpty(url))
                        .OfType<string>()
                        .ToList();

                    if (imageUrls.Any())
                    {
                        await _fileStorageService.DeleteMultipleAsync(imageUrls);
                    }

                    product.RemoveVariant(variantToDelete);
                }

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
                        var existingVariant = product.Variants.FirstOrDefault(v => v.Id == variantDto.Id);
                        if (existingVariant != null)
                        {
                            existingVariant.UpdatePrice(variantDto.Price, _currentUserService.UserId);
                            existingVariant.UpdateStock(variantDto.Stock, _currentUserService.UserId);
                            existingVariant.UpdateDiscountPercent(variantDto.DiscountPercent, _currentUserService.UserId);
                        }
                    }
                    else
                    {
                        var sku = new SKU(variantDto.Sku);
                        var newVariant = new Variant(
                            createBy: _currentUserService.UserId,
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
                                createBy: _currentUserService.UserId,
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

            return product.ToDto();
        }

        /// <summary>
        /// حذف منطقی محصول و حذف فیزیکی تمام تصاویر مرتبط (اصلی و واریانت‌ها)
        /// </summary>
        /// <param name="id">شناسه محصول برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                throw new NotFoundException($"محصول با شناسه {id} یافت نشد.", DomainConcept.Product, nameof(Product.Id));

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

            product.Delete(_currentUserService.UserId);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// فعال‌سازی محصول
        /// </summary>
        /// <param name="id">شناسه محصول برای فعال‌سازی</param>
        public async Task ActivateAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                throw new NotFoundException(
                    $"محصول با شناسه {id} یافت نشد.",
                    DomainConcept.Product,
                    nameof(Product.Id));

            product.Activate(_currentUserService.UserId);
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// غیرفعال‌سازی محصول
        /// </summary>
        /// <param name="id">شناسه محصول برای غیرفعال‌سازی</param>
        public async Task DeactivateAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                throw new NotFoundException(
                    $"محصول با شناسه {id} یافت نشد.",
                    DomainConcept.Product, nameof(Product.Id));

            product.Deactivate(_currentUserService.UserId);
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تنظیم یک تصویر خاص به عنوان تصویر اصلی (کاور) محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <param name="imageId">شناسه تصویر</param>
        public async Task SetMainImageAsync(long productId, long imageId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
                throw new NotFoundException($"محصول با شناسه {productId} یافت نشد.", DomainConcept.Product, nameof(Product.Id));

            product.SetMainImage(imageId, _currentUserService.UserId);

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
                throw new NotFoundException($"محصول با شناسه {productId} یافت نشد.", DomainConcept.Product, nameof(Product.Id));

            var variant = product.Variants.FirstOrDefault(v => v.Id == variantId);

            if (variant is null)
                throw new NotFoundException($"واریانت با شناسه {variantId} در این محصول یافت نشد.", DomainConcept.Variant, nameof(Variant.Id));

            var imageUrls = variant.ProductImages
                .Select(img => img.ImageUrl?.Value)
                .Where(url => !string.IsNullOrEmpty(url))
                .OfType<string>()
                .ToList();

            if (imageUrls.Any())
            {
                await _fileStorageService.DeleteMultipleAsync(imageUrls);
            }

            product.RemoveVariant(variant);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }
    }
}