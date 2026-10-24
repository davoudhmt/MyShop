namespace Application.DTOs.Identity.Auth
{
    /// <summary>
    /// DTO برای تأیید کد OTP و ورود به سیستم
    /// </summary>
    public class VerifyOtpDto
    {
        /// <summary>
        /// شماره موبایل کاربر
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// کد تأیید وارد شده توسط کاربر
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }
}