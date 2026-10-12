using Application.DTOs.Catalog.Tag;
using FluentValidation;

namespace Application.Validators.Catalog.Tag;

/// <summary>
/// اعتبارسنجی DTO ساخت برچسب جدید
/// قوانین این Validator دقیقاً منطبق با قوانین موجودیت Tag در لایه Domain است
/// </summary>
public class CreateTagValidator : AbstractValidator<CreateTagDto>
{
    /// <summary>
    /// سازنده و تعریف قوانین اعتبارسنجی برای ساخت برچسب
    /// </summary>
    public CreateTagValidator()
    {
        // نام برچسب: اجباری، بین 2 تا 50 کاراکتر (مطابق دامنه)
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام برچسب نمی‌تواند خالی باشد.")
            .Length(2, 50).WithMessage("نام برچسب باید بین 2 تا 50 کاراکتر باشد.");
    }
}