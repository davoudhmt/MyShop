using Application.DTOs.Identity.Auth;
using FluentValidation;

namespace Application.Validators.Identity.Auth
{
    /// <summary>
    /// اعتبارسنجی DTO تأیید کد OTP
    /// </summary>
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpDto>
    {
        /// <summary>
        /// سازنده اعتبارسنج و تعریف قوانین
        /// </summary>
        public VerifyOtpValidator()
        {
            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("شماره موبایل نمی‌تواند خالی باشد.")
                .Length(11).WithMessage("شماره موبایل باید دقیقاً ۱ رقم باشد.")
                .Matches("^09[0-9]{9}$").WithMessage("فرمت شماره موبایل نامعتبر است.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("کد تأیید نمی‌تواند خالی باشد.")
                .Length(5).WithMessage("کد تأیید باید دقیقاً ۵ رقم باشد.")
                .Matches("^[0-9]{5}$").WithMessage("کد تأیید فقط می‌تواند شامل اعداد باشد.");
        }
    }
}