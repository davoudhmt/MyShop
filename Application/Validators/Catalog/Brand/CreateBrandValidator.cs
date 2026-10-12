using Application.DTOs.Catalog.Brand;
using Application.Extensions;
using FluentValidation;

namespace Application.Validators.Catalog.Brand;

/// <summary>
/// اعتبارسنجی DTO ساخت برند جدید
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت Brand در لایه Domain است
/// </summary>
public class CreateBrandValidator : AbstractValidator<CreateBrandDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای ساخت برند
    /// </summary>
    public CreateBrandValidator()
    {
        // نام برند: اجباری، بین 2 تا 100 کاراکتر (مطابق دامنه)
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام برند نمی‌تواند خالی باشد.")
            .Length(2, 100).WithMessage("نام برند باید بین 2 تا 100 کاراکتر باشد.");

        // توضیحات: اختیاری، اما اگر پر شد حداکثر 500 کاراکتر
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("توضیحات برند نمی‌تواند بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // آدرس تصویر: اختیاری، اما باید URL معتبر و پسوند تصویری داشته باشد
        RuleFor(x => x.ImageUrl)
            .MustBeValidImageUrl()
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }
}