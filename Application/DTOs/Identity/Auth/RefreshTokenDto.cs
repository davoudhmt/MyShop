namespace Application.DTOs.Identity.Auth
{
    /// <summary>
    /// داده‌های ورودی برای درخواست نوسازی توکن (Refresh Token)
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// شناسه کاربر (معمولاً از Access Token منقضی شده استخراج می‌شود)
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// توکن نوسازی فعلی
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }
}