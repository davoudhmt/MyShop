using Application.DTOs.Catalog.ProductImage;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Catalog;
using Application.Mappings.Catalog;
using Domain.Constants;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت تصاویر محصول و واریانت
    /// </summary>
    public class ProductImageService : IProductImageService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IProductImageRepository _imageRepository;
        private readonly IFileStorageService _fileStorageService;

        /// <summary>
        /// سازنده سرویس تصویر محصول
        /// </summary>
        /// <param name="imageRepository">ریپازیتوری تصویر محصول</param>
        /// <param name="fileStorageService">سرویس مدیریت فایل برای حذف فیزیکی</param>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        public ProductImageService(
            IProductImageRepository imageRepository,
            IFileStorageService fileStorageService,
            ICurrentUserService currentUserService)
        {
            _imageRepository = imageRepository;
            _fileStorageService = fileStorageService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// دریافت تصویر با شناسه
        /// </summary>
        /// <param name="id">شناسه تصویر</param>
        /// <returns>اطلاعات تصویر یا null در صورت عدم وجود</returns>
        public async Task<ProductImageDto?> GetByIdAsync(long id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            return image?.ToDto();
        }

        /// <summary>
        /// دریافت تمام تصاویر مرتبط با یک محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <returns>لیست تصاویر محصول به صورت DTO</returns>
        public async Task<List<ProductImageDto>> GetByProductIdAsync(long productId)
        {
            var images = await _imageRepository.GetByProductIdAsync(productId);
            return images.ToDtoList(); // ← استفاده از Extension Method
        }

        /// <summary>
        /// دریافت تصویر اصلی (کاور) یک محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <returns>تصویر اصلی یا null در صورت عدم وجود</returns>
        public async Task<ProductImageDto?> GetMainImageByProductIdAsync(long productId)
        {
            var image = await _imageRepository.GetMainImageByProductIdAsync(productId);
            return image?.ToDto();
        }

        /// <summary>
        /// دریافت تمام تصاویر مرتبط با یک واریانت خاص
        /// </summary>
        /// <param name="variantId">شناسه واریانت</param>
        /// <returns>لیست تصاویر واریانت به صورت DTO</returns>
        public async Task<List<ProductImageDto>> GetByVariantIdAsync(long variantId)
        {
            var images = await _imageRepository.GetByVariantIdAsync(variantId);
            return images.ToDtoList();
        }

        /// <summary>
        /// ساخت تصویر جدید برای محصول یا واریانت
        /// </summary>
        /// <param name="dto">اطلاعات تصویر جدید</param>
        /// <returns>تصویر ساخته شده به صورت DTO</returns>
        public async Task<ProductImageDto> CreateAsync(CreateProductImageDto dto)
        {
            var image = new ProductImage(
                createBy: _currentUserService.UserId,
                imageUrl: new ImageUrl(dto.ImageUrl),
                productId: dto.ProductId,
                variantId: dto.VariantId,
                isMain: dto.IsMain,
                altText: dto.AltText);

            await _imageRepository.AddAsync(image);
            await _imageRepository.SaveChangesAsync();

            return image.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات یک تصویر موجود
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده</param>
        /// <returns>تصویر به‌روزرسانی شده به صورت DTO</returns>
        public async Task<ProductImageDto> UpdateAsync(UpdateProductImageDto dto)
        {
            var image = await _imageRepository.GetByIdAsync(dto.Id);
            if (image is null)
            {
                throw new NotFoundException(
                    $"تصویر با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.ProductImage,
                    nameof(ProductImage.Id));
            }

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                image.UpdateImageUrl(new ImageUrl(dto.ImageUrl), _currentUserService.UserId);

            if (dto.AltText != null)
                image.UpdateAltText(dto.AltText, _currentUserService.UserId);

            if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active) image.Activate(_currentUserService.UserId);
                else if (status == EntityStatus.Inactive) image.Inactivate(_currentUserService.UserId);
            }

            _imageRepository.Update(image);
            await _imageRepository.SaveChangesAsync();

            return image.ToDto();
        }

        /// <summary>
        /// حذف منطقی تصویر و حذف فیزیکی فایل از سرور
        /// </summary>
        /// <param name="id">شناسه تصویر برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image is null)
            {
                throw new NotFoundException(
                    $"تصویر با شناسه {id} یافت نشد.",
                    DomainConcept.ProductImage,
                    nameof(ProductImage.Id));
            }

            if (!string.IsNullOrEmpty(image.ImageUrl?.Value))
            {
                await _fileStorageService.DeleteAsync(image.ImageUrl.Value);
            }

            image.Delete(_currentUserService.UserId);

            _imageRepository.Update(image);
            await _imageRepository.SaveChangesAsync();
        }
    }
}