using Application.DTOs.Identity.Auth;
using Application.DTOs.Identity.User;

namespace Application.Interfaces.Services.Identity
{
    /// <summary>
    /// واسط سرویس احراز هویت و مدیریت نشست (Authentication)
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// درخواست کد تأیید (OTP) برای شماره موبایل
        /// </summary>
        /// <param name="dto">شامل شماره موبایل</param>
        Task RequestOtpAsync(RequestOtpDto dto);

        /// <summary>
        /// تأیید کد OTP و ورود به سیستم
        /// </summary>
        /// <param name="dto">شامل شماره موبایل و کد تأیید</param>
        Task<TokenResultDto> VerifyOtpAsync(VerifyOtpDto dto);

        /// <summary>
        /// نوسازی توکن دسترسی و توکن نوسازی (Refresh Token Rotation)
        /// </summary>
        /// <param name="dto">شناسه کاربر و توکن نوسازی فعلی</param>
        /// <returns>شامل توکن دسترسی و توکن نوسازی جدید</returns>
        Task<TokenResultDto> RefreshTokenAsync(RefreshTokenDto dto);

        /// <summary>
        /// دریافت اطلاعات پروفایل کاربر احراز هویت شده‌ی جاری
        /// </summary>
        Task<UserProfileDto> GetCurrentProfileAsync();
    }
}