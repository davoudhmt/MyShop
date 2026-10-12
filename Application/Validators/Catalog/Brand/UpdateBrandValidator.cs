using Application.DTOs.Catalog.Brand;
using Application.Extensions;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Catalog.Brand;

/// <summary>
/// اعتبارسنجی DTO به‌روزرسانی برند
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت Brand در لایه Domain است
/// </summary>
public class UpdateBrandValidator : AbstractValidator<UpdateBrandDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای به‌روزرسانی برند
    /// </summary>
    public UpdateBrandValidator()
    {
        // شناسه: باید معتبر (بزرگتر از صفر) باشد
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("شناسه برند نامعتبر است.");

        // نام برند: همان قوانین Create
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام برند نمی‌تواند خالی باشد.")
            .Length(2, 100).WithMessage("نام برند باید بین 2 تا 100 کاراکتر باشد.");

        // توضیحات: همان قوانین Create
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("توضیحات برند نمی‌تواند بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // آدرس تصویر: همان قوانین Create
        RuleFor(x => x.ImageUrl)
            .MustBeValidImageUrl()
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        // وضعیت: باید یکی از مقادیر معتبر EntityStatus باشد
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("وضعیت برند نمی‌تواند خالی باشد.")
            .Must(BeValidStatus).WithMessage("وضعیت برند باید 'Active' یا 'Inactive' باشد.");
    }

    /// <summary>
    /// بررسی معتبر بودن مقدار وضعیت (تطبیق با Enum EntityStatus)
    /// </summary>
    /// <param name="status">مقدار رشته‌ای وضعیت</param>
    /// <returns>true اگر معتبر باشد، در غیر این صورت false</returns>
    private bool BeValidStatus(string status) => Enum.TryParse<EntityStatus>(status, true, out _);
}