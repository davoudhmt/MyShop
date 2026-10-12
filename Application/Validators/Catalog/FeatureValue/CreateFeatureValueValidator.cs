using Application.DTOs.Catalog.FeatureValue;
using FluentValidation;

namespace Application.Validators.Catalog.FeatureValue;

/// <summary>
/// اعتبارسنجی DTO ساخت مقدار ویژگی جدید
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت FeatureValue در لایه Domain است
/// </summary>
public class CreateFeatureValueValidator : AbstractValidator<CreateFeatureValueDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای ساخت مقدار ویژگی
    /// </summary>
    public CreateFeatureValueValidator()
    {
        // مقدار ویژگی: اجباری، بین 1 تا 100 کاراکتر (مطابق دامنه)
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("مقدار ویژگی نمی‌تواند خالی باشد.")
            .Length(1, 100).WithMessage("مقدار ویژگی باید بین 1 تا 100 کاراکتر باشد.");

        // شناسه ویژگی: باید معتبر (بزرگتر از صفر) باشد
        RuleFor(x => x.FeatureId)
            .GreaterThan(0).WithMessage("شناسه ویژگی نامعتبر است.");
    }
}