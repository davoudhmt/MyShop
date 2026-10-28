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
        /// درخواست کد تأیید (OTP) مخصوص کاربران دارای نقش سیستمی (ادمین)
        /// اگر کاربر نقش سیستمی نداشته باشد، پیام موفقیت کاذب نمایش داده می‌شود (جلوگیری از User Enumeration)
        /// </summary>
        /// <param name="dto">شامل شماره موبایل</param>
        Task RequestAdminOtpAsync(RequestOtpDto dto);

        /// <summary>
        /// تأیید کد OTP ادمین و صدور توکن موقت (StepToken)
        /// </summary>
        /// <param name="dto">شامل شماره موبایل و کد تأیید</param>
        /// <returns>شامل توکن موقت و زمان انقضای آن</returns>
        Task<StepTokenResultDto> VerifyAdminOtpAsync(VerifyOtpDto dto);

        /// <summary>
        /// ورود نهایی ادمین با استفاده از رمز عبور و توکن موقت معتبر
        /// </summary>
        /// <param name="dto">شامل شماره موبایل، رمز عبور و توکن موقت</param>
        /// <returns>شامل توکن دسترسی و توکن نوسازی اصلی</returns>
        Task<TokenResultDto> AdminPasswordLoginAsync(AdminPasswordLoginDto dto);

        /// <summary>
        /// نوسازی توکن دسترسی و توکن نوسازی (Refresh Token Rotation)
        /// </summary>
        /// <param name="dto">شناسه کاربر و توکن نوسازی فعلی</param>
        /// <returns>شامل توکن دسترسی و توکن نوسازی جدید</returns>
        Task<TokenResultDto> RefreshTokenAsync(RefreshTokenDto dto);

        /// <summary>
        /// خروج از سیستم و باطل کردن توکن نوسازی جاری
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// دریافت اطلاعات پروفایل کاربر احراز هویت شده‌ی جاری
        /// </summary>
        Task<UserProfileDto> GetCurrentProfileAsync();
    }
}