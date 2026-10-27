using API.Settings;
using Application.DTOs.Identity.Auth;
using Application.Interfaces.Services.Identity;
using Domain.Entities.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    /// <summary>
    /// پیاده‌سازی سرویس تولید توکن‌های احراز هویت با استفاده از استاندارد JWT
    /// </summary>
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// سازنده سرویس توکن با تزریق تنظیمات JWT
        /// </summary>
        /// <param name="jwtOptions">تنظیمات JWT از appsettings</param>
        public JwtTokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// تولید توکن دسترسی (Access Token) و توکن نوسازی (Refresh Token)
        /// </summary>
        /// <param name="user">موجودیت کاربر</param>
        /// <returns>شامل توکن دسترسی، توکن نوسازی و زمان انقضای آن‌ها</returns>
        public Task<TokenResultDto> GenerateTokensAsync(User user)
        {
            // ۱. تعریف Claimهای کاربر (فقط با استفاده از اطلاعات موجودیت User)
            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),               // شناسه استاندارد کاربر (Subject)
                new Claim(ClaimTypes.MobilePhone, user.Username)    // شماره موبایل (که همان Username است)
            };

            // ۲. ساخت کلید امنیتی برای امضای توکن
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

            // ۳. تولید توکن JWT
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: accessTokenExpiration,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // ۴. تولید Refresh Token امن (یک رشته تصادفی ۳۲ بایتی به فرمت Base64)
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            var result = new TokenResultDto
            {
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            return Task.FromResult(result);
        }

        /// <summary>
        /// تولید یک رشته تصادفی امن برای Refresh Token
        /// </summary>
        /// <returns>رشته تصادفی رمزنگاری شده</returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}