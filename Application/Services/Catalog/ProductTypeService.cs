using Application.DTOs.Catalog.ProductType;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس نوع محصول
    /// </summary>
    public class ProductTypeService : IProductTypeService
    {
        /// <summary>
        /// شناسه کاربر سیستم (موقتاً ثابت)
        /// </summary>
        private const long SystemUser = 1;

        private readonly IProductTypeRepository _productTypeRepository;

        /// <summary>
        /// سازنده سرویس نوع محصول
        /// </summary>
        /// <param name="productTypeRepository">ریپازیتوری نوع محصول</param>
        public ProductTypeService(IProductTypeRepository productTypeRepository)
        {
            _productTypeRepository = productTypeRepository;
        }

        /// <summary>
        /// دریافت همه انواع محصول
        /// </summary>
        public async Task<List<ProductTypeDto>> GetAllAsync()
        {
            var productTypes = await _productTypeRepository.GetAllAsync();
            return productTypes.Select(ToDto).ToList();
        }

        /// <summary>
        /// دریافت نوع محصول با شناسه
        /// </summary>
        public async Task<ProductTypeDto?> GetByIdAsync(long id)
        {
            var productType = await _productTypeRepository.GetByIdAsync(id);
            return productType is null ? null : ToDto(productType);
        }

        /// <summary>
        /// دریافت انواع محصول یک دسته‌بندی خاص
        /// </summary>
        public async Task<List<ProductTypeDto>> GetByCategoryIdAsync(long categoryId)
        {
            var productTypes = await _productTypeRepository.GetByCategoryIdAsync(categoryId);
            return productTypes.Select(ToDto).ToList();
        }

        /// <summary>
        /// دریافت انواع محصول با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<ProductTypeDto>> GetPagedAsync(ProductTypeFilterDto filter)
        {
            var result = await _productTypeRepository.GetPagedAsync(filter);

            return new PagedResult<ProductTypeDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// ساخت نوع محصول جدید
        /// </summary>
        public async Task<ProductTypeDto> CreateAsync(CreateProductTypeDto dto)
        {
            // ===== بررسی تکراری بودن =====
            if (await _productTypeRepository.ExistsByNameAsync(dto.Name.Trim()))
                throw new InvalidOperationException($"نوع محصول با نام '{dto.Name}' قبلاً ثبت شده است.");

            // ===== ساخت موجودیت =====
            var productType = new ProductType(
                createBy: SystemUser,
                name: dto.Name,
                description: dto.Description);

            await _productTypeRepository.AddAsync(productType);
            await _productTypeRepository.SaveChangesAsync();

            return ToDto(productType);
        }

        /// <summary>
        /// به‌روزرسانی نوع محصول
        /// </summary>
        public async Task<ProductTypeDto> UpdateAsync(UpdateProductTypeDto dto)
        {
            var productType = await _productTypeRepository.GetByIdAsync(dto.Id);
            if (productType is null)
                throw new InvalidOperationException($"نوع محصول با شناسه {dto.Id} یافت نشد.");

            // ===== بررسی تکراری بودن (به جز خودش) =====
            var existingByName = await _productTypeRepository.GetByNameAsync(dto.Name.Trim());
            if (existingByName is not null && existingByName.Id != dto.Id)
                throw new InvalidOperationException($"نوع محصول با نام '{dto.Name}' قبلاً ثبت شده است.");

            // ===== به‌روزرسانی فیلدها =====
            productType.UpdateProductTypeName(dto.Name, SystemUser);
            productType.UpdateProductTypeDescription(dto.Description, SystemUser);

            // ===== تغییر وضعیت =====
            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    productType.Activate(SystemUser);
                else if (status == EntityStatus.Inactive)
                    productType.Inactivate(SystemUser);
            }

            _productTypeRepository.Update(productType);
            await _productTypeRepository.SaveChangesAsync();

            return ToDto(productType);
        }

        /// <summary>
        /// حذف منطقی نوع محصول
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            var productType = await _productTypeRepository.GetByIdAsync(id);
            if (productType is null)
                throw new InvalidOperationException($"نوع محصول با شناسه {id} یافت نشد.");

            productType.Delete(SystemUser);

            _productTypeRepository.Update(productType);
            await _productTypeRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تبدیل موجودیت به DTO
        /// </summary>
        private static ProductTypeDto ToDto(ProductType productType)
        {
            return new ProductTypeDto
            {
                Id = productType.Id,
                Name = productType.Name,
                Description = productType.Description,
                Status = productType.Status.ToString(),
                CreateDate = productType.CreateDate,
                UpdateDate = productType.UpdateDate
            };
        }
    }
}