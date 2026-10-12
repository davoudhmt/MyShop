using Application.DTOs.Catalog.Tag;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Catalog.Tag;

/// <summary>
/// اعتبارسنجی DTO به‌روزرسانی برچسب
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت Tag در لایه Domain است
/// </summary>
public class UpdateTagValidator : AbstractValidator<UpdateTagDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای به‌روزرسانی برچسب
    /// </summary>
    public UpdateTagValidator()
    {
        // شناسه: باید معتبر (بزرگتر از صفر) باشد
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("شناسه برچسب نامعتبر است.");

        // نام برچسب: همان قوانین Create
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام برچسب نمی‌تواند خالی باشد.")
            .Length(2, 50).WithMessage("نام برچسب باید بین 2 تا 50 کاراکتر باشد.");

        // وضعیت: باید یکی از مقادیر معتبر EntityStatus باشد
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("وضعیت برچسب نمی‌تواند خالی باشد.")
            .Must(BeValidStatus).WithMessage("وضعیت برچسب باید 'Active' یا 'Inactive' باشد.");
    }

    /// <summary>
    /// بررسی معتبر بودن مقدار وضعیت (تطبیق با Enum EntityStatus)
    /// </summary>
    /// <param name="status">مقدار رشته‌ای وضعیت</param>
    /// <returns>true اگر معتبر باشد، در غیر این صورت false</returns>
    private bool BeValidStatus(string status) => Enum.TryParse<EntityStatus>(status, true, out _);
}