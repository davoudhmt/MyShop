using Application.DTOs.Catalog.FeatureValue;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Catalog.FeatureValue;

/// <summary>
/// اعتبارسنجی DTO به‌روزرسانی مقدار ویژگی
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت FeatureValue در لایه Domain است
/// </summary>
public class UpdateFeatureValueValidator : AbstractValidator<UpdateFeatureValueDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای به‌روزرسانی مقدار ویژگی
    /// </summary>
    public UpdateFeatureValueValidator()
    {
        // شناسه: باید معتبر (بزرگتر از صفر) باشد
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("شناسه مقدار ویژگی نامعتبر است.");

        // مقدار ویژگی: همان قوانین Create
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("مقدار ویژگی نمی‌تواند خالی باشد.")
            .Length(1, 100).WithMessage("مقدار ویژگی باید بین 1 تا 100 کاراکتر باشد.");

        // وضعیت: باید یکی از مقادیر معتبر EntityStatus باشد
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("وضعیت مقدار ویژگی نمی‌تواند خالی باشد.")
            .Must(BeValidStatus).WithMessage("وضعیت مقدار ویژگی باید 'Active' یا 'Inactive' باشد.");
    }

    /// <summary>
    /// بررسی معتبر بودن مقدار وضعیت (تطبیق با Enum EntityStatus)
    /// </summary>
    /// <param name="status">مقدار رشته‌ای وضعیت</param>
    /// <returns>true اگر معتبر باشد، در غیر این صورت false</returns>
    private bool BeValidStatus(string status) => Enum.TryParse<EntityStatus>(status, true, out _);
}