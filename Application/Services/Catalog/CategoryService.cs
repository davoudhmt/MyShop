using Application.DTOs.Catalog.Category;
using Application.DTOs.Common;
using Application.Interfaces.Repositories.Catalog;
using Application.Interfaces.Services.Catalog;
using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Services.Catalog
{
    /// <summary>
    /// پیاده‌سازی سرویس دسته‌بندی
    /// </summary>
    public class CategoryService : ICategoryService
    {
        /// <summary>
        /// شناسه کاربر فعلی (موقتاً ثابت؛ بعداً از احراز هویت واقعی می‌گیریم)
        /// </summary>
        private const long SystemUser = 1;

        private readonly ICategoryRepository _categoryRepository;

        /// <summary>
        /// سازنده سرویس دسته‌بندی
        /// </summary>
        /// <param name="categoryRepository">ریپازیتوری دسته‌بندی</param>
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// دریافت همه دسته‌بندی‌ها
        /// </summary>
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(ToDto).ToList();
        }

        /// <summary>
        /// دریافت دسته‌بندی با شناسه
        /// </summary>
        public async Task<CategoryDto?> GetByIdAsync(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category is null ? null : ToDto(category);
        }

        /// <summary>
        /// دریافت دسته‌بندی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        public async Task<PagedResult<CategoryDto>> GetPagedAsync(CategoryFilterDto filter)
        {
            var result = await _categoryRepository.GetPagedAsync(filter);

            return new PagedResult<CategoryDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        /// <summary>
        /// دریافت زیردسته‌های یک دسته
        /// </summary>
        public async Task<List<CategoryDto>> GetChildrenAsync(long parentCategoryId)
        {
            var children = await _categoryRepository.GetChildrenAsync(parentCategoryId);
            return children.Select(ToDto).ToList();
        }

        /// <summary>
        /// ساخت دسته‌بندی جدید
        /// </summary>
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            // ===== بررسی تکراری بودن =====
            if (await _categoryRepository.ExistsByNameAsync(dto.Name.Trim()))
                throw new InvalidOperationException($"دسته‌بندی با نام '{dto.Name}' قبلاً ثبت شده است.");

            if (await _categoryRepository.ExistsByCodeAsync(dto.CategoryCode.Trim()))
                throw new InvalidOperationException($"دسته‌بندی با کد '{dto.CategoryCode}' قبلاً ثبت شده است.");

            // ===== ساخت موجودیت =====
            var category = new Category(
                createBy: SystemUser,
                name: dto.Name,
                categoryCode: new CategoryCode(dto.CategoryCode),
                description: dto.Description,
                imageUrl: string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : new ImageUrl(dto.ImageUrl));

            // ===== اگه والد داشت، از AddChild استفاده کن تا قوانین عمق چک بشه =====
            if (dto.ParentCategoryId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);
                if (parent is null)
                    throw new InvalidOperationException($"دسته‌بندی والد با شناسه {dto.ParentCategoryId} یافت نشد.");

                parent.AddChild(category);
                _categoryRepository.Update(parent);
            }

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return ToDto(category);
        }

        /// <summary>
        /// به‌روزرسانی دسته‌بندی
        /// </summary>
        public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.Id);
            if (category is null)
                throw new InvalidOperationException($"دسته‌بندی با شناسه {dto.Id} یافت نشد.");

            // ===== بررسی تکراری بودن (به جز خودش) =====
            var existingByName = await _categoryRepository.GetByNameAsync(dto.Name.Trim());
            if (existingByName is not null && existingByName.Id != dto.Id)
                throw new InvalidOperationException($"دسته‌بندی با نام '{dto.Name}' قبلاً ثبت شده است.");

            var existingByCode = await _categoryRepository.GetByCodeAsync(dto.CategoryCode.Trim());
            if (existingByCode is not null && existingByCode.Id != dto.Id)
                throw new InvalidOperationException($"دسته‌بندی با کد '{dto.CategoryCode}' قبلاً ثبت شده است.");

            // ===== به‌روزرسانی فیلدها =====
            category.UpdateCategoryName(dto.Name, SystemUser);
            category.UpdateCategoryCode(new CategoryCode(dto.CategoryCode), SystemUser);
            category.UpdateCategoryDescription(dto.Description, SystemUser);
            category.UpdateCategoryImageUrl(
                string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : new ImageUrl(dto.ImageUrl),
                SystemUser);

            // ===== تغییر والد (با چک چرخه و عمق) =====
            if (dto.ParentCategoryId != category.ParentCategoryId)
            {
                if (dto.ParentCategoryId is null)
                {
                    category.MakeRootCategory(SystemUser);
                }
                else
                {
                    var newParent = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);
                    if (newParent is null)
                        throw new InvalidOperationException($"دسته‌بندی والد با شناسه {dto.ParentCategoryId} یافت نشد.");

                    category.MoveTo(newParent, SystemUser);
                }
            }

            // ===== تغییر وضعیت =====
            if (Enum.TryParse<EntityStatus>(dto.Status, true, out var status))
            {
                if (status == EntityStatus.Active)
                    category.Activate(SystemUser);
                else if (status == EntityStatus.Inactive)
                    category.Inactivate(SystemUser);
            }

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();

            return ToDto(category);
        }

        /// <summary>
        /// حذف منطقی دسته‌بندی
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
                throw new InvalidOperationException($"دسته‌بندی با شناسه {id} یافت نشد.");

            category.Delete(SystemUser);

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }

        /// <summary>
        /// تبدیل موجودیت به DTO
        /// </summary>
        private static CategoryDto ToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl?.Value,
                CategoryCode = category.CategoryCode.Value,
                ParentCategoryId = category.ParentCategoryId,
                Status = category.Status.ToString(),
                CreateDate = category.CreateDate,
                UpdateDate = category.UpdateDate
            };
        }
    }
}