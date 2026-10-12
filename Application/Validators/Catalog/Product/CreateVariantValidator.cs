using Application.DTOs.Catalog.Variant;
using Application.Extensions;
using FluentValidation;

namespace Application.Validators.Catalog.Product;

/// <summary>
/// اعتبارسنجی DTO ساخت واریانت جدید
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت Variant و Value Object SKU در لایه Domain است
/// </summary>
public class CreateVariantValidator : AbstractValidator<CreateVariantDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای ساخت واریانت
    /// </summary>
    public CreateVariantValidator()
    {
        // قیمت: نمی‌تواند منفی باشد (مطابق دامنه)
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("قیمت واریانت نمی‌تواند منفی باشد.");

        // موجودی: نمی‌تواند منفی باشد (مطابق دامنه)
        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("موجودی واریانت نمی‌تواند منفی باشد.");

        // درصد تخفیف: بین 0 تا 100 (مطابق دامنه)
        RuleFor(x => x.DiscountPercent)
            .InclusiveBetween(0, 100).WithMessage("درصد تخفیف باید بین 0 تا 100 باشد.")
            .When(x => x.DiscountPercent.HasValue);

        // کد SKU: دقیقاً 10 کاراکتر (3 حرف + 7 رقم) مطابق با Value Object SKU
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("کد SKU واریانت نمی‌تواند خالی باشد.")
            .Length(10, 10).WithMessage("کد SKU باید دقیقاً 10 کاراکتر باشد.")
            .Matches("^[a-zA-Z]{3}[0-9]{7}$").WithMessage("کد SKU باید شامل 3 حرف انگلیسی و 7 رقم باشد.");

        // شناسه مقادیر ویژگی: هر شناسه باید معتبر باشد
        RuleForEach(x => x.FeatureValueIds)
            .GreaterThan(0).WithMessage("شناسه مقدار ویژگی نامعتبر است.");

        // آدرس تصاویر: هر URL باید معتبر و دارای پسوند تصویری باشد
        RuleForEach(x => x.ImageUrls)
            .MustBeValidImageUrl()
            .When(x => x.ImageUrls != null && x.ImageUrls.Any());
    }
}