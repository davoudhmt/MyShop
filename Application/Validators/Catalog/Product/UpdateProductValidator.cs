using Application.DTOs.Catalog.Product;
using FluentValidation;

namespace Application.Validators.Catalog.Product;

/// <summary>
/// اعتبارسنجی DTO به‌روزرسانی محصول
/// </summary>
public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    /// <summary>
    /// سازنده اعتبارسنجی که قوانین مختلف برای فیلدهای DTO به‌روزرسانی محصول را تعریف می‌کند
    /// </summary>
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("شناسه محصول نامعتبر است.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام محصول نمی‌تواند خالی باشد.")
            .Length(2, 200).WithMessage("نام محصول باید بین 2 تا 200 کاراکتر باشد.");

        RuleFor(x => x.ShortDescription)
            .NotEmpty().WithMessage("توضیحات مختصر نمی‌تواند خالی باشد.")
            .MaximumLength(500).WithMessage("توضیحات مختصر نمی‌تواند بیش از 500 کاراکتر باشد.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("توضیحات کامل محصول نمی‌تواند بیش از 2000 کاراکتر باشد.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("شناسه دسته‌بندی نامعتبر است.");
        RuleFor(x => x.ProductTypeId).GreaterThan(0).WithMessage("شناسه نوع محصول نامعتبر است.");

        RuleFor(x => x.BrandId)
            .GreaterThan(0).WithMessage("شناسه برند نامعتبر است.")
            .When(x => x.BrandId.HasValue);

        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("کد محصول نمی‌تواند خالی باشد.")
            .Length(6, 6).WithMessage("کد محصول باید دقیقاً 6 کاراکتر باشد.")
            .Matches("^[a-zA-Z]{3}[0-9]{3}$").WithMessage("کد محصول باید شامل 3 حرف انگلیسی و 3 رقم باشد.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("اسلاگ نمی‌تواند خالی باشد.")
            .MaximumLength(200).WithMessage("اسلاگ نمی‌تواند بیش از 200 کاراکتر باشد.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("اسلاگ فقط می‌تواند شامل حروف کوچک، اعداد و خط تیره باشد و نمی‌تواند با خط تیره شروع یا تمام شود.");

        RuleForEach(x => x.TagIds).GreaterThan(0).WithMessage("شناسه برچسب نامعتبر است.");

        // اعتبارسنجی تو در تو (Nested Validation)
        RuleFor(x => x.Seo)
            .SetValidator(new UpdateSeoValidator());

        RuleForEach(x => x.Variants)
            .SetValidator(new UpdateVariantValidator())
            .When(x => x.Variants != null && x.Variants.Any());
    }
}