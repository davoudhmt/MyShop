using Application.DTOs.Catalog.Category;
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
    /// پیاده‌سازی سرویس مدیریت دسته‌بندی
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileStorageService _fileStorageService;

        /// <summary>
        /// سازنده سرویس دسته‌بندی
        /// </summary>
        /// <param name="categoryRepository">ریپازیتوری دسته‌بندی</param>
        /// <param name="fileStorageService">سرویس ذخیره‌سازی فایل</param>
        /// <param name="currentUserService">سرویس کاربر جاری</param>
        public CategoryService(
            ICategoryRepository categoryRepository,
            IFileStorageService fileStorageService,
            ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _fileStorageService = fileStorageService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// دریافت همه دسته‌بندی‌ها
        /// </summary>
        /// <returns>لیست تمام دسته‌بندی‌ها به صورت DTO</returns>
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.ToDtoList();
        }

        /// <summary>
        /// دریافت دسته‌بندی با شناسه
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        /// <returns>اطلاعات دسته‌بندی یا null در صورت عدم وجود</returns>
        public async Task<CategoryDto?> GetByIdAsync(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category?.ToDto();
        }

        /// <summary>
        /// دریافت دسته‌بندی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر و صفحه‌بندی</param>
        /// <returns>نتایج صفحه‌بندی شده به صورت DTO</returns>
        public async Task<PagedResult<CategoryDto>> GetPagedAsync(CategoryFilterDto filter)
        {
            var result = await _categoryRepository.GetPagedAsync(filter);

            return new PagedResult<CategoryDto>
            {
                Items = result.Items.ToDtoList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// دریافت زیردسته‌های یک دسته‌بندی خاص
        /// </summary>
        /// <param name="parentCategoryId">شناسه دسته‌بندی والد</param>
        /// <returns>لیست زیردسته‌ها به صورت DTO</returns>
        public async Task<List<CategoryDto>> GetChildrenAsync(long parentCategoryId)
        {
            var children = await _categoryRepository.GetChildrenAsync(parentCategoryId);
            return children.ToDtoList();
        }

        /// <summary>
        /// ساخت دسته‌بندی جدید
        /// </summary>
        /// <param name="dto">اطلاعات دسته‌بندی جدید</param>
        /// <returns>دسته‌بندی ساخته شده به صورت DTO</returns>
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            if (await _categoryRepository.ExistsByNameAsync(dto.Name.Trim()))
            {
                throw new ConflictException(
                    $"دسته‌بندی با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.Category,
                    nameof(Category.Name));
            }

            if (await _categoryRepository.ExistsByCodeAsync(dto.CategoryCode.Trim()))
            {
                throw new ConflictException(
                    $"دسته‌بندی با کد '{dto.CategoryCode}' قبلاً ثبت شده است.",
                    DomainConcept.CategoryCode,
                    nameof(Category.CategoryCode));
            }

            var category = new Category(
                createBy: _currentUserService.UserId,
                name: dto.Name,
                categoryCode: new CategoryCode(dto.CategoryCode),
                description: dto.Description,
                imageUrl: string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : new ImageUrl(dto.ImageUrl));

            if (dto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);

                if (parent is null)
                {
                    throw new NotFoundException(
                        $"دسته‌بندی والد با شناسه {dto.ParentCategoryId} یافت نشد.",
                        DomainConcept.Category,
                        nameof(Category.ParentCategoryId));
                }

                parent.AddChild(category);
                _categoryRepository.Update(parent);
            }

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return category.ToDto();
        }

        /// <summary>
        /// به‌روزرسانی دسته‌بندی
        /// </summary>
        /// <param name="dto">اطلاعات به‌روزرسانی شده</param>
        /// <returns>دسته‌بندی به‌روزرسانی شده به صورت DTO</returns>
        public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.Id);
            if (category is null)
            {
                throw new NotFoundException(
                    $"دسته‌بندی با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.Category,
                    nameof(Category.Id));
            }

            var existingByName = await _categoryRepository.GetByNameAsync(dto.Name.Trim());
            if (existingByName is not null && existingByName.Id != dto.Id)
            {
                throw new ConflictException(
                    $"دسته‌بندی با نام '{dto.Name}' قبلاً ثبت شده است.",
                    DomainConcept.Category,
                    nameof(Category.Name));
            }

            var existingByCode = await _categoryRepository.GetByCodeAsync(dto.CategoryCode.Trim());
            if (existingByCode is not null && existingByCode.Id != dto.Id)
            {
                throw new ConflictException(
                    $"دسته‌بندی با کد '{dto.CategoryCode}' قبلاً ثبت شده است.",
                    DomainConcept.CategoryCode,
                    nameof(Category.CategoryCode));
            }

            category.UpdateCategoryName(dto.Name, _currentUserService.UserId);
            category.UpdateCategoryCode(new CategoryCode(dto.CategoryCode), _currentUserService.UserId);
            category.UpdateCategoryDescription(dto.Description, _currentUserService.UserId);
            category.UpdateCategoryImageUrl(
                string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : new ImageUrl(dto.ImageUrl),
                _currentUserService.UserId);

            if (dto.ParentCategoryId != category.ParentCategoryId)
            {
                if (dto.ParentCategoryId is null)
                {
                    category.MakeRootCategory(_currentUserService.UserId);
                }
                else
                {
                    var newParent = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);

                    if (newParent is null)
                    {
                        throw new NotFoundException(
                            $"دسته‌بندی والد با شناسه {dto.ParentCategoryId} یافت نشد.",
                            DomainConcept.Category,
                            nameof(Category.ParentCategoryId));
                    }

                    category.MoveTo(newParent, _currentUserService.UserId);
                }
            }

            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    category.Activate(_currentUserService.UserId);
                else if (status == EntityStatus.Inactive)
                    category.Deactivate(_currentUserService.UserId);
            }

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();

            return category.ToDto();
        }

        /// <summary>
        /// حذف منطقی دسته‌بندی
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی برای حذف</param>
        public async Task DeleteAsync(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                throw new NotFoundException(
                    $"دسته‌بندی با شناسه {id} یافت نشد.",
                    DomainConcept.Category,
                    nameof(Category.Id));
            }

            if (!string.IsNullOrEmpty(category.ImageUrl?.Value))
            {
                await _fileStorageService.DeleteAsync(category.ImageUrl.Value);
            }

            category.Delete(_currentUserService.UserId);

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }

        /// <summary>
        /// فعالسازی دسته بندی
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task ActivateAsync(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                throw new NotFoundException(
                    $"دسته بندی با شناسه {id} یافت نشد.",
                    DomainConcept.Category,
                    nameof(Category.Id));
            }
            category.Activate(_currentUserService.UserId);
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }

        /// <summary>
        /// غیرفعالسازی دسته بندی
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task DeactivateAsync(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                throw new NotFoundException(
                    $"دسته بندی با شناسه {id} یافت نشد.",
                    DomainConcept.Category,
                    nameof(Category.Id));
            }
            category.Deactivate(_currentUserService.UserId);
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}