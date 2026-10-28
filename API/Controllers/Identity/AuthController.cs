using API.Middlewares;
using Application.DTOs.Identity.Auth;
using Application.DTOs.Identity.User;
using Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Identity
{
    /// <summary>
    /// کنترلر مدیریت احراز هویت و نشست‌های کاربری
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// سازنده کنترلر با تزریق سرویس احراز هویت
        /// </summary>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// درخواست کد تأیید (OTP) برای ورود یا ثبت‌نام (کاربران عادی)
        /// </summary>
        [HttpPost("request-otp")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto)
        {
            await _authService.RequestOtpAsync(dto);
            return Ok(new { message = "کد تأیید با موفقیت ارسال شد." });
        }

        /// <summary>
        /// تأیید کد OTP و ورود به سیستم (کاربران عادی)
        /// </summary>
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(TokenResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var tokenResult = await _authService.VerifyOtpAsync(dto);
            return Ok(tokenResult);
        }

        /// <summary>
        /// درخواست کد تأیید (OTP) مخصوص ورود به پنل مدیریت
        /// (در صورتی که کاربر نقش مدیریتی نداشته باشد، پیام موفقیت کاذب نمایش داده می‌شود)
        /// </summary>
        [HttpPost("request-admin-otp")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestAdminOtp([FromBody] RequestOtpDto dto)
        {
            await _authService.RequestAdminOtpAsync(dto);
            // همیشه پیام موفقیت برمی‌گردانیم تا از User Enumeration جلوگیری شود
            return Ok(new { message = "در صورت وجود حساب مدیریتی، کد تأیید ارسال شد." });
        }

        /// <summary>
        /// تأیید کد OTP ادمین و دریافت توکن موقت (StepToken)
        /// </summary>
        [HttpPost("verify-admin-otp")]
        [ProducesResponseType(typeof(StepTokenResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyAdminOtp([FromBody] VerifyOtpDto dto)
        {
            var stepTokenResult = await _authService.VerifyAdminOtpAsync(dto);
            return Ok(stepTokenResult);
        }

        /// <summary>
        /// ورود نهایی به پنل مدیریت با استفاده از رمز عبور و توکن موقت
        /// </summary>
        [HttpPost("admin-login")]
        [ProducesResponseType(typeof(TokenResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AdminLogin([FromBody] AdminPasswordLoginDto dto)
        {
            var tokenResult = await _authService.AdminPasswordLoginAsync(dto);
            return Ok(tokenResult);
        }

        /// <summary>
        /// نوسازی توکن دسترسی و توکن نوسازی (Refresh Token Rotation)
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(TokenResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var tokenResult = await _authService.RefreshTokenAsync(dto);
            return Ok(tokenResult);
        }

        /// <summary>
        /// خروج از سیستم و باطل کردن توکن نوسازی جاری
        /// </summary>
        [HttpPost("logout")]
        [Authorize] // فقط کاربران لاگین‌شده می‌توانند خارج شوند
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "با موفقیت از سیستم خارج شدید." });
        }

        /// <summary>
        /// دریافت اطلاعات پروفایل کاربر احراز هویت شده‌ی جاری
        /// </summary>
        [HttpGet("profile")]
        [Authorize] // فقط کاربران لاگین‌شده دسترسی دارند
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentProfile()
        {
            var profile = await _authService.GetCurrentProfileAsync();
            return Ok(profile);
        }
    }
}