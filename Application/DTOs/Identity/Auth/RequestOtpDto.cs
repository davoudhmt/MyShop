namespace Application.DTOs.Identity.Auth
{
    /// <summary>
    /// DTO برای درخواست کد تأیید (OTP)
    /// </summary>
    public class RequestOtpDto
    {
        /// <summary>
        /// شماره موبایل کاربر
        /// </summary>
        public string Mobile { get; set; } = string.Empty;
    }
}