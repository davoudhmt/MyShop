using API.Constants;
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
                new Claim(JwtClaimConstants.Subject, user.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.Username)
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
        /// تولید توکن موقت (StepToken) برای احراز هویت دو مرحله‌ای ادمین
        /// این توکن عمر کوتاهی (۵ دقیقه) دارد و فقط برای عبور از مرحله OTP به مرحله رمز عبور استفاده می‌شود
        /// </summary>
        /// <param name="user">موجودیت کاربر</param>
        /// <returns>رشته توکن موقت (JWT)</returns>
        public Task<string> GenerateStepTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimConstants.Subject, user.Id.ToString()),
                new Claim(JwtClaimConstants.Mobile, user.Username),
                new Claim(JwtClaimConstants.Purpose, JwtClaimConstants.PurposeAdmin2faPending)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // عمر مفید توکن موقت: ۵ دقیقه
            var expiration = DateTime.UtcNow.AddMinutes(5);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        /// <summary>
        /// اعتبارسنجی توکن موقت (StepToken)
        /// </summary>
        /// <param name="stepToken">توکن موقت دریافتی از کلاینت</param>
        /// <param name="mobile">شماره موبایلی که در مرحله قبل وارد شده است</param>
        /// <returns>شناسه کاربر (UserId) در صورت معتبر بودن توکن</returns>
        /// <exception cref="UnauthorizedAccessException">در صورت نامعتبر یا منقضی بودن توکن</exception>
        public Task<long> ValidateStepTokenAsync(string stepToken, string mobile)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // عدم تحمل تأخیر زمانی برای امنیت بیشتر
            };

            try
            {
                var principal = tokenHandler.ValidateToken(stepToken, validationParameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                var purpose = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtClaimConstants.Purpose)?.Value;
                var tokenMobile = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtClaimConstants.Mobile)?.Value;
                var userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtClaimConstants.Subject)?.Value;

                // ۱. بررسی هدف توکن
                if (purpose != JwtClaimConstants.PurposeAdmin2faPending)
                    throw new UnauthorizedAccessException("توکن موقت نامعتبر است.");

                // ۲. بررسی تطابق شماره موبایل
                if (tokenMobile != mobile)
                    throw new UnauthorizedAccessException("توکن موقت با شماره موبایل مطابقت ندارد.");

                // ۳. استخراج و بررسی شناسه کاربر
                if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out long userId))
                    throw new UnauthorizedAccessException("شناسه کاربر در توکن موقت نامعتبر است.");

                return Task.FromResult(userId);
            }
            catch (SecurityTokenException)
            {
                throw new UnauthorizedAccessException("توکن موقت منقضی شده یا نامعتبر است.");
            }
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