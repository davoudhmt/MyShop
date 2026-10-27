namespace Application.Interfaces.Services.Identity
{
    /// <summary>
    /// واسط سرویس مدیریت کش کد تأیید (OTP)
    /// این سرویس برای ذخیره و بازیابی موقت کدهای OTP استفاده می‌شود و قابلیت جایگزینی آسان با Redis را دارد
    /// </summary>
    public interface IOtpCacheService
    {
        /// <summary>
        /// ذخیره کد تأیید در کش با زمان انقضای مشخص
        /// </summary>
        /// <param name="mobile">شماره موبایل کاربر</param>
        /// <param name="otpCode">کد تأیید تولید شده</param>
        /// <param name="expiration">مدت زمان اعتبار کد</param>
        Task SetOtpAsync(string mobile, string otpCode, TimeSpan expiration);

        /// <summary>
        /// دریافت کد تأیید از کش بر اساس شماره موبایل
        /// </summary>
        /// <param name="mobile">شماره موبایل کاربر</param>
        /// <returns>کد تأیید یا null در صورت عدم وجود یا انقضا</returns>
        Task<string?> GetOtpAsync(string mobile);

        /// <summary>
        /// حذف کد تأیید از کش (پس از استفاده موفق)
        /// </summary>
        /// <param name="mobile">شماره موبایل کاربر</param>
        Task RemoveOtpAsync(string mobile);
    }
}