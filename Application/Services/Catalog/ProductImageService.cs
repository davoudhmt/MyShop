using Application.DTOs.Catalog.ProductImage;
using Application.Interfaces.Services.Catalog;
using Application.Interfaces.Repositories.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس تصویر محصول
    /// </summary>
    public class ProductImageService : IProductImageService
    {
        private const long SystemUser = 1;
        private readonly IProductImageRepository _imageRepository;

        public ProductImageService(IProductImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<ProductImageDto?> GetByIdAsync(long id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            return image is null ? null : MapToDto(image);
        }

        public async Task<List<ProductImageDto>> GetByProductIdAsync(long productId)
        {
            var images = await _imageRepository.GetByProductIdAsync(productId);
            return images.Select(MapToDto).ToList();
        }

        public async Task<ProductImageDto?> GetMainImageByProductIdAsync(long productId)
        {
            var image = await _imageRepository.GetMainImageByProductIdAsync(productId);
            return image is null ? null : MapToDto(image);
        }

        public async Task<List<ProductImageDto>> GetByVariantIdAsync(long variantId)
        {
            var images = await _imageRepository.GetByVariantIdAsync(variantId);
            return images.Select(MapToDto).ToList();
        }

        public async Task<ProductImageDto> CreateAsync(CreateProductImageDto dto)
        {
            var image = new ProductImage(
                createBy: SystemUser,
                imageUrl: new ImageUrl(dto.ImageUrl),
                productId: dto.ProductId,
                variantId: dto.VariantId,
                isMain: dto.IsMain,
                altText: dto.AltText);

            await _imageRepository.AddAsync(image);
            await _imageRepository.SaveChangesAsync();

            return MapToDto(image);
        }

        public async Task<ProductImageDto> UpdateAsync(UpdateProductImageDto dto)
        {
            var image = await _imageRepository.GetByIdAsync(dto.Id);
            if (image is null)
                throw new InvalidOperationException($"تصویر با شناسه {dto.Id} یافت نشد.");

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                image.UpdateImageUrl(new ImageUrl(dto.ImageUrl), SystemUser);

            if (dto.AltText != null)
                image.UpdateAltText(dto.AltText, SystemUser);

            if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active) image.Activate(SystemUser);
                else if (status == EntityStatus.Inactive) image.Inactivate(SystemUser);
            }

            _imageRepository.Update(image);
            await _imageRepository.SaveChangesAsync();

            return MapToDto(image);
        }

        public async Task DeleteAsync(long id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image is null)
                throw new InvalidOperationException($"تصویر با شناسه {id} یافت نشد.");

            image.Delete(SystemUser);
            _imageRepository.Update(image);
            await _imageRepository.SaveChangesAsync();
        }

        private static ProductImageDto MapToDto(ProductImage image)
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
    }
}