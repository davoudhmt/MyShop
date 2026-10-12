using Application.DTOs.Catalog.Category;
using Application.Extensions;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Catalog.Category;

/// <summary>
/// اعتبارسنجی DTO به‌روزرسانی دسته‌بندی
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت Category و Value Objectهای مرتبط در لایه Domain است
/// </summary>
public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای به‌روزرسانی دسته‌بندی
    /// </summary>
    public UpdateCategoryValidator()
    {
        // شناسه: باید معتبر باشد
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("شناسه دسته‌بندی نامعتبر است.");

        // نام: همان قوانین Create
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام دسته‌بندی نمی‌تواند خالی باشد.")
            .Length(2, 100).WithMessage("نام دسته‌بندی باید بین 2 تا 100 کاراکتر باشد.");

        // کد دسته‌بندی: همان قوانین Create (مطابق با Value Object CategoryCode)
        RuleFor(x => x.CategoryCode)
            .NotEmpty().WithMessage("کد دسته‌بندی نمی‌تواند خالی باشد.")
            .Length(3, 3).WithMessage("کد دسته‌بندی باید دقیقاً ۳ کاراکتر باشد.")
            .Matches("^[a-zA-Z]{3}$").WithMessage("کد دسته‌بندی باید فقط شامل ۳ حرف انگلیسی باشد.");

        // توضیحات: همان قوانین Create
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("توضیحات دسته‌بندی نمی‌تواند بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // تصویر: همان قوانین Create
        RuleFor(x => x.ImageUrl)
            .MustBeValidImageUrl()
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        // والد: باید معتبر باشد و نمی‌تواند خودش باشد (Fail-Fast برای تجربه کاربری بهتر)
        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0).WithMessage("شناسه دسته‌بندی والد نامعتبر است.")
            .NotEqual(x => x.Id).WithMessage("دسته‌بندی نمی‌تواند والد خودش باشد.")
            .When(x => x.ParentCategoryId.HasValue);

        // وضعیت: باید یکی از مقادیر معتبر EntityStatus باشد
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("وضعیت دسته‌بندی نمی‌تواند خالی باشد.")
            .Must(BeValidStatus).WithMessage("وضعیت دسته‌بندی باید 'Active' یا 'Inactive' باشد.");
    }

    /// <summary>
    /// بررسی معتبر بودن مقدار وضعیت (تطبیق با Enum EntityStatus)
    /// </summary>
    /// <param name="status">مقدار رشته‌ای وضعیت</param>
    /// <returns>true اگر معتبر باشد، در غیر این صورت false</returns>
    private bool BeValidStatus(string status) => Enum.TryParse<EntityStatus>(status, true, out _);
}