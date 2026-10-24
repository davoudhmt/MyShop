using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Settings;
using Application.Interfaces.Services;
using Domain.Entities.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
        public JwtTokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// تولید توکن دسترسی (Access Token) و توکن نوسازی (Refresh Token)
        /// </summary>
        public Task<TokenResult> GenerateTokensAsync(User user, Person person)
        {
            // ۱. تعریف Claimهای کاربر (اطلاعاتی که داخل توکن ذخیره می‌شوند)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.PersonId.ToString()), // شناسه کاربر برای CurrentUserService
                new Claim(ClaimTypes.MobilePhone, person.Mobile.Value),          // شماره موبایل
                // در آینده می‌توانیم Claimهای نقش (Role) را هم اینجا اضافه کنیم
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

            var result = new TokenResult
            {
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
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}