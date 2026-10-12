using Application.DTOs.Catalog.ProductType;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Catalog.ProductType;

/// <summary>
/// اعتبارسنجی DTO به‌روزرسانی نوع محصول
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت ProductType در لایه Domain است
/// </summary>
public class UpdateProductTypeValidator : AbstractValidator<UpdateProductTypeDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای به‌روزرسانی نوع محصول
    /// </summary>
    public UpdateProductTypeValidator()
    {
        // شناسه: باید معتبر (بزرگتر از صفر) باشد
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("شناسه نوع محصول نامعتبر است.");

        // نام نوع محصول: همان قوانین Create
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام نوع محصول نمی‌تواند خالی باشد.")
            .Length(2, 100).WithMessage("نام نوع محصول باید بین 2 تا 100 کاراکتر باشد.");

        // توضیحات: همان قوانین Create
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("توضیحات نوع محصول نمی‌تواند بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // وضعیت: باید یکی از مقادیر معتبر EntityStatus باشد
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("وضعیت نوع محصول نمی‌تواند خالی باشد.")
            .Must(BeValidStatus).WithMessage("وضعیت نوع محصول باید 'Active' یا 'Inactive' باشد.");
    }

    /// <summary>
    /// بررسی معتبر بودن مقدار وضعیت (تطبیق با Enum EntityStatus)
    /// </summary>
    /// <param name="status">مقدار رشته‌ای وضعیت</param>
    /// <returns>true اگر معتبر باشد، در غیر این صورت false</returns>
    private bool BeValidStatus(string status) => Enum.TryParse<EntityStatus>(status, true, out _);
}