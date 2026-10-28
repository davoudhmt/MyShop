namespace API.Constants
{
    /// <summary>
    /// ثابت‌های مرتبط با Claimهای توکن JWT
    /// این کلاس برای جلوگیری از استفاده از رشته‌های جادویی (Magic Strings) در کد طراحی شده است
    /// </summary>
    public static class JwtClaimConstants
    {
        /// <summary>
        /// نام Claim برای شناسه کاربر (Subject)
        /// </summary>
        public const string Subject = "sub";

        /// <summary>
        /// نام Claim برای شماره موبایل
        /// </summary>
        public const string Mobile = "mobile";

        /// <summary>
        /// نام Claim برای هدف توکن (Purpose)
        /// </summary>
        public const string Purpose = "purpose";

        /// <summary>
        /// مقدار Claim هدف برای توکن موقت ورود ادمین (احراز هویت دو مرحله‌ای)
        /// </summary>
        public const string PurposeAdmin2faPending = "admin_2fa_pending";
    }
}