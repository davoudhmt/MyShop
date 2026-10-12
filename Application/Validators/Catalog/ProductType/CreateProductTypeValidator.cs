using Application.DTOs.Catalog.ProductType;
using FluentValidation;

namespace Application.Validators.Catalog.ProductType;

/// <summary>
/// اعتبارسنجی DTO ساخت نوع محصول جدید
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت ProductType در لایه Domain است
/// </summary>
public class CreateProductTypeValidator : AbstractValidator<CreateProductTypeDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای ساخت نوع محصول
    /// </summary>
    public CreateProductTypeValidator()
    {
        // نام نوع محصول: اجباری، بین 2 تا 100 کاراکتر (مطابق دامنه)
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام نوع محصول نمی‌تواند خالی باشد.")
            .Length(2, 100).WithMessage("نام نوع محصول باید بین 2 تا 100 کاراکتر باشد.");

        // توضیحات: اختیاری، اما اگر پر شد حداکثر 500 کاراکتر
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("توضیحات نوع محصول نمی‌تواند بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}