using Application.DTOs.Catalog.Seo;
using FluentValidation;

namespace Application.Validators.Catalog.Product;

/// <summary>
/// اعتبارسنجی DTO ساخت تنظیمات سئو
/// قوانین دقیقاً منطبق با Value Object Seo در لایه Domain است
/// </summary>
public class CreateSeoValidator : AbstractValidator<CreateSeoDto?>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای سئو
    /// </summary>
    public CreateSeoValidator()
    {
        RuleFor(x => x!.Title)
            .MaximumLength(200).WithMessage("عنوان سئو نمی‌تواند بیش از 200 کاراکتر باشد.")
            .When(x => x?.Title != null);

        RuleFor(x => x!.H1)
            .MaximumLength(200).WithMessage("تگ H1 نمی‌تواند بیش از 200 کاراکتر باشد.")
            .When(x => x?.H1 != null);

        RuleFor(x => x!.Description)
            .MaximumLength(500).WithMessage("توضیحات سئو نمی‌تواند بیش از 500 کاراکتر باشد.")
            .When(x => x?.Description != null);

        RuleFor(x => x!.MetaDescription)
            .MaximumLength(300).WithMessage("متا دسکریپشن نمی‌تواند بیش از 300 کاراکتر باشد.")
            .When(x => x?.MetaDescription != null);

        RuleFor(x => x!.MetaKeywords)
            .MaximumLength(300).WithMessage("متا کیوردز نمی‌تواند بیش از 300 کاراکتر باشد.")
            .When(x => x?.MetaKeywords != null);
    }
}