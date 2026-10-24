using Domain.Entities.Identity;

namespace Application.Interfaces.Services
{
    /// <summary>
    /// واسط سرویس تولید و مدیریت توکن‌های احراز هویت
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// تولید توکن دسترسی (Access Token) و توکن نوسازی (Refresh Token) برای کاربر
        /// </summary>
        /// <param name="user">موجودیت کاربر</param>
        /// <param name="person">موجودیت شخص مرتبط با کاربر</param>
        /// <returns>شامل توکن دسترسی، توکن نوسازی و زمان انقضای آن‌ها</returns>
        Task<TokenResult> GenerateTokensAsync(User user, Person person);
    }

    /// <summary>
    /// نتیجه‌ی تولید توکن‌ها
    /// </summary>
    public class TokenResult
    {
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