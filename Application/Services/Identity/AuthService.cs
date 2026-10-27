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
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository; // ✅ برای متدهای RequestOtp و GetCurrentProfile لازم است
        private readonly IMessageService _messageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// سازنده سرویس احراز هویت و مقداردهی وابستگی‌ها
        /// </summary>
        public AuthService(
            IUserRepository userRepository,
            IPersonRepository personRepository,
            IMessageService messageService,
            ICurrentUserService currentUserService,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
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

            if (user.IsLockedOut())
                throw new ConflictException("حساب کاربری شما به دلیل تلاش‌های ناموفق متعدد موقتاً قفل شده است.", DomainConcept.User, nameof(User));

            var otpCode = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
            var expiryTime = DateTime.UtcNow.AddMinutes(2);

            // ✅ استفاده از user.Id به عنوان updateBy
            user.GenerateOtp(otpCode, expiryTime, user.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            await _messageService.SendToMobileAsync(dto.Mobile, $"کد تأیید شما: {otpCode}\nاین کد تا ۲ دقیقه معتبر است.");
        }

        /// <summary>
        /// تأیید کد OTP و ورود به سیستم
        /// </summary>
        public async Task<TokenResultDto> VerifyOtpAsync(VerifyOtpDto dto)
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
                user.VerifyAndClearOtp(dto.Code, user.Id);
                user.RecordLogin(ipAddress: "127.0.0.1", updateBy: user.Id);

                // ✅ تولید توکن فقط با استفاده از موجودیت User (بدون نیاز به Person)
                var tokenResult = await _tokenService.GenerateTokensAsync(user);

                // ✅ ذخیره Refresh Token با استفاده از user.Id
                user.SetRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiration, user.Id);

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return tokenResult;
            }
            catch (DomainException)
            {
                user.IncrementAccessFailedCount(user.Id);
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                throw;
            }
        }

        /// <summary>
        /// نوسازی توکن دسترسی و توکن نوسازی با استفاده از الگوی چرخش توکن (Refresh Token Rotation)
        /// </summary>
        public async Task<TokenResultDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            // ۱. یافتن کاربر بر اساس توکن نوسازی
            var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);

            // ۲. بررسی وجود کاربر، تطابق شناسه و اعتبار توکن
            if (user is null || user.Id != dto.UserId || !user.IsRefreshTokenValid(dto.RefreshToken))
            {
                throw new UnauthorizedAccessException("توکن نوسازی نامعتبر است یا منقضی شده است.");
            }

            // ✅ ۳. تولید توکن‌های کاملاً جدید (فقط با استفاده از user، بدون کوئری اضافه برای Person)
            var newTokenResult = await _tokenService.GenerateTokensAsync(user);

            // ✅ ۴. اعمال چرخش توکن با استفاده از user.Id
            user.SetRefreshToken(newTokenResult.RefreshToken, newTokenResult.RefreshTokenExpiration, user.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            // ۵. بازگرداندن توکن‌های جدید به کلاینت
            return newTokenResult;
        }

        /// <summary>
        /// دریافت پروفایل کاربر جاری
        /// </summary>
        public async Task<UserProfileDto> GetCurrentProfileAsync()
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException("کاربر احراز هویت نشده است.");

            // ۱. ابتدا کاربر را بر اساس UserId (که از توکن گرفته شده) پیدا می‌کنیم
            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
            if (user is null)
                throw new NotFoundException("کاربر یافت نشد.", DomainConcept.User, nameof(User));

            // ۲. سپس با استفاده از PersonIdِ همان کاربر، اطلاعات شخص را می‌گیریم
            var person = await _personRepository.GetByIdAsync(user.PersonId);
            if (person is null)
                throw new NotFoundException("اطلاعات شخص یافت نشد.", DomainConcept.Person, nameof(Person));

            return person.ToDto();
        }
    }
}