using Application.DTOs.Identity.Auth;
using Domain.Entities.Identity;

namespace Application.Interfaces.Services.Identity
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
        /// <returns>شامل توکن دسترسی، توکن نوسازی و زمان انقضای آن‌ها</returns>
        Task<TokenResultDto> GenerateTokensAsync(User user);
    }
}