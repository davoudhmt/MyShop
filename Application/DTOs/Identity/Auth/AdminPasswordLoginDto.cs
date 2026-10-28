namespace Application.DTOs.Identity.Auth
{
    /// <summary>
    /// داده‌های ورودی برای مرحله سوم و نهایی ورود ادمین: ارسال رمز عبور به همراه توکن موقت (StepToken)
    /// </summary>
    public class AdminPasswordLoginDto
    {
        /// <summary>
        /// شماره موبایل ادمین
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// رمز عبور ادمین
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// توکن موقت (StepToken) دریافت شده از مرحله تأیید OTP
        /// این توکن یک JWT کوتاه‌مدت است که ثابت می‌کند ادمین مرحله OTP را با موفقیت گذرانده است
        /// </summary>
        public string StepToken { get; set; } = string.Empty;
    }
}