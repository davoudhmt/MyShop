using Application.DTOs.Identity.Permission;
using FluentValidation;

namespace Application.Validators.Identity.Permission
{
    /// <summary>
    /// اعتبارسنجی DTO ایجاد مجوز جدید
    /// </summary>
    public class CreatePermissionValidator : AbstractValidator<CreatePermissionDto>
    {
        public CreatePermissionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام مجوز الزامی است.")
                .MinimumLength(3).WithMessage("نام مجوز باید حداقل 3 کاراکتر باشد.")
                .MaximumLength(100).WithMessage("نام مجوز نمی‌تواند بیش از 100 کاراکتر باشد.");

            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("نام نمایشی الزامی است.")
                .MinimumLength(3).WithMessage("نام نمایشی باید حداقل 3 کاراکتر باشد.")
                .MaximumLength(100).WithMessage("نام نمایشی نمی‌تواند بیش از 100 کاراکتر باشد.");

            RuleFor(x => x.Module)
                .IsInEnum().WithMessage("ماژول انتخاب‌شده نامعتبر است.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("توضیحات نمی‌تواند بیش از 500 کاراکتر باشد.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}