namespace API.Enums
{
    /// <summary>
    /// انواع خطاهای استاندارد سیستم
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// خطای مربوط به قوانین دامنه (کسب‌وکار)
        /// </summary>
        DomainError,

        /// <summary>
        /// خطای اعتبارسنجی ورودی‌ها
        /// </summary>
        ValidationError,

        /// <summary>
        /// منبع مورد نظر یافت نشد (404)
        /// </summary>
        NotFound,

        /// <summary>
        /// کاربر احراز هویت نشده است (401)
        /// </summary>
        Unauthorized,

        /// <summary>
        /// کاربر دسترسی ندارد (403)
        /// </summary>
        Forbidden,

        /// <summary>
        /// خطای داخلی و پیش‌بینی‌نشده سرور (500)
        /// </summary>
        InternalError
    }
}