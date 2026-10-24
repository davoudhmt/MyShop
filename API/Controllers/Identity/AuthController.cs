using API.Middlewares;
using Application.DTOs.Identity.Auth;
using Application.DTOs.Identity.User;
using Application.Interfaces.Services;
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
        /// <param name="authService">سرویس احراز هویت</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// درخواست کد تأیید (OTP) برای ورود یا ثبت‌نام
        /// </summary>
        /// <param name="dto">شماره موبایل کاربر</param>
        /// <returns>پیام موفقیت</returns>
        [HttpPost("request-otp")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto)
        {
            await _authService.RequestOtpAsync(dto);
            return Ok(new { message = "کد تأیید با موفقیت ارسال شد." });
        }

        /// <summary>
        /// تأیید کد OTP و ورود به سیستم
        /// </summary>
        /// <param name="dto">شماره موبایل و کد تأیید</param>
        /// <returns>پیام موفقیت (و در آینده توکن JWT)</returns>
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(TokenResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            // ✅ دریافت توکن‌ها از سرویس
            var tokenResult = await _authService.VerifyOtpAsync(dto);

            // ✅ بازگرداندن توکن‌ها به کلاینت
            return Ok(tokenResult);
        }

        /// <summary>
        /// دریافت اطلاعات پروفایل کاربر احراز هویت شده‌ی جاری
        /// </summary>
        /// <returns>اطلاعات پروفایل کاربر</returns>
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