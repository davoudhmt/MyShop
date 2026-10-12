using Application.DTOs.Catalog.Category;
using Application.Extensions;
using FluentValidation;

namespace Application.Validators.Catalog.Category;

/// <summary>
/// اعتبارسنجی DTO ساخت دسته‌بندی جدید
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت Category و Value Objectهای مرتبط (CategoryCode, ImageUrl) در لایه Domain است
/// </summary>
public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای ساخت دسته‌بندی
    /// </summary>
    public CreateCategoryValidator()
    {
        // نام: اجباری، بین 2 تا 100 کاراکتر (مطابق دامنه)
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام دسته‌بندی نمی‌تواند خالی باشد.")
            .Length(2, 100).WithMessage("نام دسته‌بندی باید بین 2 تا 100 کاراکتر باشد.");

        // کد دسته‌بندی: دقیقاً ۳ حرف انگلیسی (مطابق با Value Object CategoryCode)
        RuleFor(x => x.CategoryCode)
            .NotEmpty().WithMessage("کد دسته‌بندی نمی‌تواند خالی باشد.")
            .Length(3, 3).WithMessage("کد دسته‌بندی باید دقیقاً ۳ کاراکتر باشد.")
            .Matches("^[a-zA-Z]{3}$").WithMessage("کد دسته‌بندی باید فقط شامل ۳ حرف انگلیسی باشد.");

        // توضیحات: اختیاری، حداکثر 500 کاراکتر
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("توضیحات دسته‌بندی نمی‌تواند بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // تصویر: اختیاری، اما باید URL معتبر و پسوند تصویری داشته باشد
        RuleFor(x => x.ImageUrl)
            .MustBeValidImageUrl()
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        // والد: اگر مقدار داشت، باید بزرگتر از صفر باشد
        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0).WithMessage("شناسه دسته‌بندی والد نامعتبر است.")
            .When(x => x.ParentCategoryId.HasValue);
    }
}