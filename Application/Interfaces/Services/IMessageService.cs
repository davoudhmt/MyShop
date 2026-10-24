namespace Application.Interfaces.Services
{
    /// <summary>
    /// واسط سرویس ارسال پیام (انتزاعی برای پشتیبانی از SMS، ایمیل و سایر پلتفرم‌ها)
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// ارسال پیام به شماره موبایل
        /// </summary>
        /// <param name="mobile">شماره موبایل مقصد</param>
        /// <param name="message">متن پیام</param>
        Task SendToMobileAsync(string mobile, string message);
    }
}