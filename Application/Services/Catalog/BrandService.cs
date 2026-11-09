using Application.DTOs.Catalog.Brand;
using Application.DTOs.Common;
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
    /// پیاده‌سازی سرویس مدیریت برند
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IBrandRepository _brandRepository;
        private readonly IFileStorageService _fileStorageService;

        /// <summary>
        /// سازنده کلاس سرویس برند
        /// </summary>
        /// <param name="brandRepository">ریپازیتوری برند</param>
        /// <param name="fileStorageService">سرویس مدیریت فایل</param>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        public BrandService(
            IBrandRepository brandRepository,
            IFileStorageService fileStorageService,
            ICurrentUserService currentUserService)
        {
            _brandRepository = brandRepository;
            _fileStorageService = fileStorageService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// دریافت همه برندها
        /// </summary>
        public async Task<List<BrandDto>> GetAllAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return brands.ToDtoList();
        }

        /// <summary>
        /// دریافت برندها با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        public async Task<PagedResult<BrandDto>> GetPagedAsync(BrandFilterDto filter)
        {
            var pagedBrands = await _brandRepository.GetPagedAsync(filter);

            return new PagedResult<BrandDto>
            {
                Items = pagedBrands.Items.ToDtoList(),
                PageNumber = pagedBrands.PageNumber,
                PageSize = pagedBrands.PageSize,
                TotalCount = pagedBrands.TotalCount
            };
        }

        /// <summary>
        /// دریافت برند با شناسه
        /// </summary>
        public async Task<BrandDto?> GetByIdAsync(long id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return brand?.ToDto();
        }

        /// <summary>
        /// ساخت برند جدید
        /// </summary>
        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            if (await _brandRepository.ExistsByNameAsync(dto.Name.Trim()))
            {
                throw new ConflictException(
                    $"برند با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.Brand,
                    nameof(Brand.Name));
            }

            // تبدیل ImageUrl با استفاده از TryCreate
            ImageUrl? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                if (!ImageUrl.TryCreate(dto.ImageUrl, out imageUrl))
                {
                    throw new DomainException(
                        "آدرس تصویر برند معتبر نیست.",
                        DomainConcept.ImageUrl,
                        nameof(Brand.ImageUrl));
                }
            }

            // ساخت موجودیت برند
            var brand = new Brand(
                createBy: _currentUserService.UserId,
                name: dto.Name,
                description: dto.Description,
                imageUrl: imageUrl);

            // ذخیره در دیتابیس
            await _brandRepository.AddAsync(brand);
            await _brandRepository.SaveChangesAsync();

            return brand.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی برند
        /// </summary>
        public async Task UpdateAsync(UpdateBrandDto dto)
        {
            var brand = await _brandRepository.GetByIdAsync(dto.Id);
            if (brand is null)
            {
                throw new NotFoundException(
                    $"برند با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.Brand,
                    nameof(Brand.Id));
            }

            // تبدیل ImageUrl
            ImageUrl? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                if (!ImageUrl.TryCreate(dto.ImageUrl, out imageUrl))
                {
                    throw new DomainException(
                        "آدرس تصویر برند معتبر نیست.",
                        DomainConcept.ImageUrl,
                        nameof(Brand.ImageUrl));
                }
            }

            // به‌روزرسانی اطلاعات
            brand.UpdateBrandName(dto.Name, _currentUserService.UserId);
            brand.UpdateBrandDescription(dto.Description, _currentUserService.UserId);
            brand.UpdateBrandImageUrl(imageUrl, _currentUserService.UserId);

            // تغییر وضعیت
            if (dto.Status == nameof(EntityStatus.Active))
                brand.Activate(_currentUserService.UserId);
            else if (dto.Status == nameof(EntityStatus.Inactive))
                brand.Deactivate(_currentUserService.UserId);

            // ذخیره در دیتابیس
            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// حذف برند
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null)
            {
                throw new NotFoundException(
                    $"برند با شناسه {id} یافت نشد.",
                    DomainConcept.Brand,
                    nameof(Brand.Id));
            }

            if (!string.IsNullOrEmpty(brand.ImageUrl?.Value))
            {
                await _fileStorageService.DeleteAsync(brand.ImageUrl.Value);
            }

            brand.Delete(_currentUserService.UserId);

            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// فعالسازی برند
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task ActivateAsync(long id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null)
            {
                throw new NotFoundException(
                    $"برند با شناسه {id} یافت نشد.",
                    DomainConcept.Brand,
                    nameof(Brand.Id));
            }
            brand.Activate(_currentUserService.UserId);
            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// غیرفعالسازی برند
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task DeactivateAsync(long id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null)
            {
                throw new NotFoundException(
                    $"برند با شناسه {id} یافت نشد.",
                    DomainConcept.Brand,
                    nameof(Brand.Id));
            }
            brand.Deactivate(_currentUserService.UserId);
            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync();
        }
    }
}