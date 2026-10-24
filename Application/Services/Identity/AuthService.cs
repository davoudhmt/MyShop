using Application.DTOs.Identity.Auth;
using Application.DTOs.Identity.User;
using Application.Interfaces.Repositories.Identity;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Mappings.Identity;
using Domain.Constants;
using Domain.Entities.Identity;
using Domain.Exceptions;
using System.Security.Cryptography;

namespace Application.Services.Identity
{
    /// <summary>
    /// پیاده‌سازی سرویس احراز هویت (Auth)
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageService _messageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// سازنده سرویس احراز هویت و مقداردهی وابستگی‌ها
        /// </summary>
        public AuthService(
            IPersonRepository personRepository,
            IUserRepository userRepository,
            IMessageService messageService,
            ICurrentUserService currentUserService,
            ITokenService tokenService)
        {
            _personRepository = personRepository;
            _userRepository = userRepository;
            _messageService = messageService;
            _currentUserService = currentUserService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// درخواست کد تأیید (OTP)
        /// </summary>
        public async Task RequestOtpAsync(RequestOtpDto dto)
        {
            var person = await _personRepository.GetByMobileAsync(dto.Mobile);
            if (person is null)
                throw new NotFoundException($"شخصی با شماره موبایل {dto.Mobile} یافت نشد.", DomainConcept.Person, nameof(dto.Mobile));

            var user = await _userRepository.GetByPersonIdAsync(person.Id);
            if (user is null)
                throw new NotFoundException("حساب کاربری برای این شخص یافت نشد.", DomainConcept.User, nameof(User));

            // بررسی قفل بودن حساب قبل از ارسال کد جدید
            if (user.IsLockedOut())
                throw new ConflictException("حساب کاربری شما به دلیل تلاش‌های ناموفق متعدد موقتاً قفل شده است.", DomainConcept.User, nameof(User));

            // تولید کد OTP (یک کد ۵ رقمی تصادفی)
            var otpCode = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
            var expiryTime = DateTime.UtcNow.AddMinutes(2); // انقضا پس از ۲ دقیقه

            // فراخوانی متد دامنه
            user.GenerateOtp(otpCode, expiryTime, person.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            // ارسال پیام (در محیط واقعی باید await شود)
            await _messageService.SendToMobileAsync(dto.Mobile, $"کد تأیید شما: {otpCode}\nاین کد تا ۲ دقیقه معتبر است.");
        }

        /// <summary>
        /// تأیید کد OTP و ورود به سیستم
        /// </summary>
        public async Task<TokenResult> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var person = await _personRepository.GetByMobileAsync(dto.Mobile);
            if (person is null)
                throw new NotFoundException($"شخصی با شماره موبایل {dto.Mobile} یافت نشد.", DomainConcept.Person, nameof(dto.Mobile));

            var user = await _userRepository.GetByPersonIdAsync(person.Id);
            if (user is null)
                throw new NotFoundException("حساب کاربری برای این شخص یافت نشد.", DomainConcept.User, nameof(User));

            if (user.IsLockedOut())
                throw new ConflictException("حساب کاربری شما به دلیل تلاش‌های ناموفق متعدد موقتاً قفل شده است.", DomainConcept.User, nameof(User));

            try
            {
                user.VerifyAndClearOtp(dto.Code, person.Id);
                user.RecordLogin(ipAddress: "127.0.0.1", updateBy: person.Id);

                // ✅ ۱. تولید توکن‌ها
                var tokenResult = await _tokenService.GenerateTokensAsync(user, person);

                // ✅ ۲. ذخیره Refresh Token در موجودیت کاربر (برای مدیریت نشست‌ها در آینده)
                user.SetRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiration, person.Id);

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                // ✅ ۳. بازگرداندن نتیجه توکن
                return tokenResult;
            }
            catch (DomainException)
            {
                user.IncrementAccessFailedCount(person.Id);
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                throw;
            }
        }

        /// <summary>
        /// دریافت پروفایل کاربر جاری
        /// </summary>
        public async Task<UserProfileDto> GetCurrentProfileAsync()
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException("کاربر احراز هویت نشده است.");

            var person = await _personRepository.GetByIdAsync(_currentUserService.UserId);
            if (person is null)
                throw new NotFoundException("اطلاعات شخص یافت نشد.", DomainConcept.Person, nameof(Person));

            return person.ToDto();
        }
    }
}