using Application.DTOs.Identity.Auth;
using FluentValidation;

namespace Application.Validators.Identity.Auth
{
    /// <summary>
    /// اعتبارسنجی DTO درخواست کد تأیید (OTP)
    /// </summary>
    public class RequestOtpValidator : AbstractValidator<RequestOtpDto>
    {
        /// <summary>
        /// سازنده اعتبارسنج و تعریف قوانین
        /// </summary>
        public RequestOtpValidator()
        {
            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("شماره موبایل نمی‌تواند خالی باشد.")
                .Length(11).WithMessage("شماره موبایل باید دقیقاً ۱۱ رقم باشد.")
                .Matches("^09[0-9]{9}$").WithMessage("فرمت شماره موبایل نامعتبر است. فرمت صحیح: 09xxxxxxxxx");
        }
    }
}