using Application.DTOs.Identity.Auth;
using FluentValidation;

namespace Application.Validators.Identity.Auth
{
    /// <summary>
    /// اعتبارسنجی DTO ورود نهایی ادمین (شامل رمز عبور و توکن موقت)
    /// </summary>
    public class AdminPasswordLoginValidator : AbstractValidator<AdminPasswordLoginDto>
    {
        /// <summary>
        /// سازنده اعتبارسنج و تعریف قوانین اعتبارسنجی سخت‌گیرانه برای امنیت بالا
        /// </summary>
        public AdminPasswordLoginValidator()
        {
            // اعتبارسنجی شماره موبایل
            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("شماره موبایل نمی‌تواند خالی باشد.")
                .Length(11).WithMessage("شماره موبایل باید دقیقاً ۱۱ رقم باشد.")
                .Matches("^09[0-9]{9}$").WithMessage("فرمت شماره موبایل نامعتبر است. فرمت صحیح: 09xxxxxxxxx");

            // اعتبارسنجی رمز عبور با استانداردهای امنیتی بالا
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("رمز عبور نمی‌تواند خالی باشد.")
                .MinimumLength(8).WithMessage("رمز عبور باید حداقل ۸ کاراکتر باشد.")
                .Matches("[A-Z]").WithMessage("رمز عبور باید حداقل شامل یک حرف بزرگ انگلیسی باشد.")
                .Matches("[a-z]").WithMessage("رمز عبور باید حداقل شامل یک حرف کوچک انگلیسی باشد.")
                .Matches("[^a-zA-Z0-9]").WithMessage("رمز عبور باید حداقل شامل یک کاراکتر خاص (مانند @, #, $, %, etc.) باشد.");

            // اعتبارسنجی توکن موقت (StepToken)
            RuleFor(x => x.StepToken)
                .NotEmpty().WithMessage("توکن موقت (StepToken) نمی‌تواند خالی باشد.");
        }
    }
}