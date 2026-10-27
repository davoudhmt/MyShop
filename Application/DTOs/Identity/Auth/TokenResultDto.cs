namespace Application.DTOs.Identity.Auth
{
    /// <summary>
    /// نتیجه‌ی تولید توکن‌ها
    /// </summary>
    public class TokenResultDto
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// توکن دسترسی (JWT)
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// توکن نوسازی (Refresh Token)
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// زمان انقضای توکن دسترسی
        /// </summary>
        public DateTime AccessTokenExpiration { get; set; }

        /// <summary>
        /// زمان انقضای توکن نوسازی
        /// </summary>
        public DateTime RefreshTokenExpiration { get; set; }
    }
}