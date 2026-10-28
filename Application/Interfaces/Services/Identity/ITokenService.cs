using Application.DTOs.Identity.Auth;
using Domain.Entities.Identity;

namespace Application.Interfaces.Services.Identity
{
    /// <summary>
    /// واسط سرویس تولید و مدیریت توکن‌های احراز هویت
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// تولید توکن دسترسی (Access Token) و توکن نوسازی (Refresh Token) برای کاربر
        /// </summary>
        /// <param name="user">موجودیت کاربر</param>
        /// <returns>شامل توکن دسترسی، توکن نوسازی و زمان انقضای آن‌ها</returns>
        Task<TokenResultDto> GenerateTokensAsync(User user);

        /// <summary>
        /// تولید توکن موقت (StepToken) برای احراز هویت دو مرحله‌ای ادمین
        /// این توکن عمر کوتاهی (مثلاً ۵ دقیقه) دارد و فقط برای عبور از مرحله OTP به مرحله رمز عبور استفاده می‌شود
        /// </summary>
        /// <param name="user">موجودیت کاربر</param>
        /// <returns>رشته توکن موقت (JWT)</returns>
        Task<string> GenerateStepTokenAsync(User user);

        /// <summary>
        /// اعتبارسنجی توکن موقت (StepToken)
        /// </summary>
        /// <param name="stepToken">توکن موقت دریافتی از کلاینت</param>
        /// <param name="mobile">شماره موبایلی که در مرحله قبل وارد شده است</param>
        /// <returns>شناسه کاربر (UserId) در صورت معتبر بودن توکن</returns>
        Task<long> ValidateStepTokenAsync(string stepToken, string mobile);
    }
}