using Application.DTOs.Catalog.Brand;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Services.Catalog
{
    /// <summary>
    /// سرویس برند
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private const long SystemUser = 1;  // کاربر سیستم (موقت)

        /// <summary>
        /// سازنده کلاس
        /// </summary>
        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        /// <summary>
        /// دریافت همه برندها
        /// </summary>
        public async Task<List<BrandDto>> GetAllAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return brands.Select(MapToDto).ToList();
        }

        /// <summary>
        /// دریافت برندها با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        public async Task<PagedResult<BrandDto>> GetPagedAsync(BrandFilterDto filter)
        {
            // دریافت داده‌های صفحه‌بندی شده از Repository
            var pagedBrands = await _brandRepository.GetPagedAsync(filter);

            // تبدیل موجودیت‌ها به DTO
            var items = pagedBrands.Items.Select(MapToDto).ToList();

            // ساخت نتیجه با DTO
            return new PagedResult<BrandDto>
            {
                Items = items,
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
            return brand is null ? null : MapToDto(brand);
        }

        /// <summary>
        /// ساخت برند جدید
        /// </summary>
        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            // بررسی وجود برند با همین نام
            var exists = await _brandRepository.ExistsByNameAsync(dto.Name);
            if (exists)
                throw new System.InvalidOperationException($"برند با نام '{dto.Name}' قبلاً ثبت شده است.");

            // تبدیل ImageUrl با استفاده از TryCreate
            ImageUrl? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                if (!ImageUrl.TryCreate(dto.ImageUrl, out imageUrl))
                    throw new System.InvalidOperationException("آدرس تصویر برند معتبر نیست.");
            }

            // ساخت موجودیت برند
            var brand = new Brand(
                createBy: SystemUser,
                name: dto.Name,
                description: dto.Description,
                imageUrl: imageUrl);

            // فعال‌سازی برند
            brand.Activate(SystemUser);

            // ذخیره در دیتابیس
            await _brandRepository.AddAsync(brand);
            await _brandRepository.SaveChangesAsync();

            return MapToDto(brand);
        }

        /// <summary>
        /// به‌روزرسانی برند
        /// </summary>
        public async Task UpdateAsync(UpdateBrandDto dto)
        {
            // دریافت برند از دیتابیس
            var brand = await _brandRepository.GetByIdAsync(dto.Id);
            if (brand is null)
                throw new System.InvalidOperationException($"برند با شناسه {dto.Id} یافت نشد.");

            // تبدیل ImageUrl
            ImageUrl? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                if (!ImageUrl.TryCreate(dto.ImageUrl, out imageUrl))
                    throw new System.InvalidOperationException("آدرس تصویر برند معتبر نیست.");
            }

            // به‌روزرسانی اطلاعات
            brand.UpdateBrandName(dto.Name, SystemUser);
            brand.UpdateBrandDescription(dto.Description, SystemUser);
            brand.UpdateBrandImageUrl(imageUrl, SystemUser);

            // تغییر وضعیت
            if (dto.Status == nameof(EntityStatus.Active))
                brand.Activate(SystemUser);
            else if (dto.Status == nameof(EntityStatus.Inactive))
                brand.Inactivate(SystemUser);

            // ذخیره در دیتابیس
            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// حذف برند
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            // دریافت برند از دیتابیس
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null)
                throw new System.InvalidOperationException($"برند با شناسه {id} یافت نشد.");

            brand.Delete(SystemUser);       // حذف منطقی
            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// نگاشت موجودیت به DTO
        /// </summary>
        private static BrandDto MapToDto(Brand brand)
        {
            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                ImageUrl = brand.ImageUrl,  // ← استفاده از implicit operator
                Status = brand.Status.ToString(),
                CreateDate = brand.CreateDate,
                UpdateDate = brand.UpdateDate
            };
        }
    }
}