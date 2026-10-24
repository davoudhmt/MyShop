namespace API.Settings
{
    /// <summary>
    /// کلاس تنظیمات JWT برای خواندن Type-Safe از appsettings.json
    /// این کلاس با الگوی Options Pattern در ASP.NET Core هماهنگ است.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// نام بخش در appsettings.json
        /// </summary>
        public const string SectionName = "Jwt";

        /// <summary>
        /// کلید مخفی برای امضای دیجیتال توکن (حداقل ۳۲ کاراکتر)
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// نام صادرکننده‌ی توکن (Issuer)
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// نام مصرف‌کننده‌ی توکن (Audience)
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// مدت اعتبار Access Token به دقیقه
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; }

        /// <summary>
        /// مدت اعتبار Refresh Token به روز
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; }
    }
}