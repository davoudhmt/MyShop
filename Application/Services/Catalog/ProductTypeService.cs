using Application.DTOs.Catalog.ProductType;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Catalog;
using Application.Mappings.Catalog;
using Domain.Constants;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت نوع محصول (Product Type)
    /// </summary>
    public class ProductTypeService : IProductTypeService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IProductTypeRepository _productTypeRepository;

        /// <summary>
        /// سازنده سرویس نوع محصول
        /// </summary>
        /// <param name="productTypeRepository">ریپازیتوری نوع محصول</param>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        public ProductTypeService(
            IProductTypeRepository productTypeRepository,
            ICurrentUserService currentUserService)
        {
            _productTypeRepository = productTypeRepository;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// دریافت همه انواع محصول
        /// </summary>
        /// <returns>لیست تمام انواع محصول به صورت DTO</returns>
        public async Task<List<ProductTypeDto>> GetAllAsync()
        {
            var productTypes = await _productTypeRepository.GetAllAsync();
            return productTypes.ToDtoList();
        }

        /// <summary>
        /// دریافت نوع محصول با شناسه
        /// </summary>
        /// <param name="id">شناسه نوع محصول</param>
        /// <returns>اطلاعات نوع محصول یا null در صورت عدم وجود</returns>
        public async Task<ProductTypeDto?> GetByIdAsync(long id)
        {
            var productType = await _productTypeRepository.GetByIdAsync(id);
            return productType?.ToDto();
        }

        /// <summary>
        /// دریافت انواع محصول یک دسته‌بندی خاص
        /// </summary>
        /// <param name="categoryId">شناسه دسته‌بندی</param>
        /// <returns>لیست انواع محصول مرتبط به صورت DTO</returns>
        public async Task<List<ProductTypeDto>> GetByCategoryIdAsync(long categoryId)
        {
            var productTypes = await _productTypeRepository.GetByCategoryIdAsync(categoryId);
            return productTypes.ToDtoList();
        }

        /// <summary>
        /// دریافت انواع محصول با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<ProductTypeDto>> GetPagedAsync(ProductTypeFilterDto filter)
        {
            var result = await _productTypeRepository.GetPagedAsync(filter);

            return new PagedResult<ProductTypeDto>
            {
                Items = result.Items.ToDtoList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// ساخت نوع محصول جدید
        /// </summary>
        /// <param name="dto">اطلاعات نوع محصول جدید</param>
        /// <returns>نوع محصول ساخته شده به صورت DTO</returns>
        public async Task<ProductTypeDto> CreateAsync(CreateProductTypeDto dto)
        {
            if (await _productTypeRepository.ExistsByNameAsync(dto.Name.Trim()))
            {
                throw new ConflictException(
                    $"نوع محصول با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.ProductType,
                    nameof(ProductType.Name));
            }

            var productType = new ProductType(
                createBy: _currentUserService.UserId,
                name: dto.Name,
                description: dto.Description);

            await _productTypeRepository.AddAsync(productType);
            await _productTypeRepository.SaveChangesAsync();

            return productType.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی نوع محصول
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده</param>
        /// <returns>نوع محصول به‌روزرسانی شده به صورت DTO</returns>
        public async Task<ProductTypeDto> UpdateAsync(UpdateProductTypeDto dto)
        {
            var productType = await _productTypeRepository.GetByIdAsync(dto.Id);
            if (productType is null)
            {
                throw new NotFoundException(
                    $"نوع محصول با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.ProductType,
                    nameof(ProductType.Id));
            }

            var existingByName = await _productTypeRepository.GetByNameAsync(dto.Name.Trim());
            if (existingByName is not null && existingByName.Id != dto.Id)
            {
                throw new ConflictException(
                    $"نوع محصول با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.ProductType,
                    nameof(ProductType.Name));
            }

            productType.UpdateProductTypeName(dto.Name, _currentUserService.UserId);
            productType.UpdateProductTypeDescription(dto.Description, _currentUserService.UserId);

            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    productType.Activate(_currentUserService.UserId);
                else if (status == EntityStatus.Inactive)
                    productType.Inactivate(_currentUserService.UserId);
            }

            _productTypeRepository.Update(productType);
            await _productTypeRepository.SaveChangesAsync();

            return productType.ToDto();
        }

        /// <summary>
        /// حذف منطقی نوع محصول
        /// </summary>
        /// <param name="id">شناسه نوع محصول برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var productType = await _productTypeRepository.GetByIdAsync(id);
            if (productType is null)
            {
                throw new NotFoundException(
                    $"نوع محصول با شناسه {id} یافت نشد.",
                    DomainConcept.ProductType,
                    nameof(ProductType.Id));
            }

            productType.Delete(_currentUserService.UserId);

            _productTypeRepository.Update(productType);
            await _productTypeRepository.SaveChangesAsync();
        }
    }
}