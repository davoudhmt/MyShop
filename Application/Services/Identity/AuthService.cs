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
    /// پیاده‌سازی سرویس احراز هویت (Auth) با رعایت اصول امنیت و Clean Architecture
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IMessageService _messageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _tokenService;
        private readonly IOtpCacheService _otpCacheService;

        /// <summary>
        /// سازنده سرویس احراز هویت و مقداردهی وابستگی‌ها
        /// </summary>
        public AuthService(
            IUserRepository userRepository,
            IPersonRepository personRepository,
            IMessageService messageService,
            ICurrentUserService currentUserService,
            ITokenService tokenService,
            IOtpCacheService otpCacheService)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
            _messageService = messageService;
            _currentUserService = currentUserService;
            _tokenService = tokenService;
            _otpCacheService = otpCacheService;
        }

        /// <summary>
        /// درخواست کد تأیید (OTP) و ذخیره آن در کش (بدون ایجاد رکورد اضافی در دیتابیس)
        /// </summary>
        /// <param name="dto">شماره موبایل کاربر</param>
        public async Task RequestOtpAsync(RequestOtpDto dto)
        {
            // تولید کد OTP (یک کد ۵ رقمی تصادفی و امن)
            var otpCode = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
            var expiration = TimeSpan.FromMinutes(2); // انقضا پس از ۲ دقیقه

            // ذخیره در کش (به جای دیتابیس، برای جلوگیری از رکوردهای یتیم)
            await _otpCacheService.SetOtpAsync(dto.Mobile, otpCode, expiration);

            // ارسال پیام (در محیط واقعی باید await شود)
            await _messageService.SendToMobileAsync(dto.Mobile, $"کد تأیید شما: {otpCode}\nاین کد تا ۲ دقیقه معتبر است.");
        }

        /// <summary>
        /// تأیید کد OTP، ساخت/یافتن کاربر و ورود به سیستم (همراه با محافظت در برابر Brute Force)
        /// </summary>
        /// <param name="dto">شماره موبایل و کد تأیید</param>
        /// <returns>شامل توکن دسترسی و توکن نوسازی</returns>
        public async Task<TokenResultDto> VerifyOtpAsync(VerifyOtpDto dto)
        {
            // ۱. بررسی اولیه: آیا شخصی با این موبایل وجود دارد؟ (برای مدیریت قفل بودن حساب)
            var person = await _personRepository.GetByMobileAsync(dto.Mobile);
            User? existingUser = null;

            if (person != null)
            {
                existingUser = await _userRepository.GetByPersonIdAsync(person.Id);
                if (existingUser != null && existingUser.IsLockedOut())
                {
                    throw new ConflictException("حساب کاربری شما به دلیل تلاش‌های ناموفق متعدد موقتاً قفل شده است.", DomainConcept.User, nameof(User));
                }
            }

            // ۲. اعتبارسنجی کد از طریق کش
            var cachedOtp = await _otpCacheService.GetOtpAsync(dto.Mobile);

            // اگر کد در کش نبود یا با کد وارد شده مطابقت نداشت
            if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.Code)
            {
                // اگر کاربر وجود دارد، تلاش ناموفق را ثبت کن (محافظت در برابر Brute Force)
                if (existingUser != null)
                {
                    existingUser.IncrementAccessFailedCount(existingUser.Id);
                    _userRepository.Update(existingUser);
                    await _userRepository.SaveChangesAsync();
                }

                // پیام خطای عمومی (جلوگیری از User Enumeration: مهاجم نمی‌فهمد کاربر هست یا نه)
                throw new UnauthorizedAccessException("کد تأیید نامعتبر یا منقضی شده است.");
            }

            // ۳. کد صحیح است! عملیات Find or Create (فقط در صورت موفقیت)
            if (person is null)
            {
                person = new Person(
                    createBy: 0, // 0 نشان‌دهنده ثبت‌نام سیستمی/ناشناس است
                    mobile: dto.Mobile
                );
                await _personRepository.AddAsync(person);
                await _personRepository.SaveChangesAsync(); // برای دریافت Person.Id
            }

            var user = await _userRepository.GetByPersonIdAsync(person.Id);
            if (user is null)
            {
                user = new User(
                    createBy: person.Id,
                    username: dto.Mobile,
                    personId: person.Id
                );
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            // ۴. پاک کردن کد از کش (جلوگیری از استفاده مجدد - Replay Attack)
            await _otpCacheService.RemoveOtpAsync(dto.Mobile);

            // ۵. ریست کردن تلاش‌های ناموفق و ثبت ورود موفق
            user.ResetAccessFailedCount(user.Id);
            user.RecordLogin(ipAddress: "127.0.0.1", updateBy: user.Id);

            // ۶. تولید توکن و ذخیره Refresh Token
            var tokenResult = await _tokenService.GenerateTokensAsync(user);
            user.SetRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiration, user.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return tokenResult;
        }

        /// <summary>
        /// نوسازی توکن دسترسی و توکن نوسازی با استفاده از الگوی چرخش توکن (Refresh Token Rotation)
        /// </summary>
        /// <param name="dto">شناسه کاربر و توکن نوسازی فعلی</param>
        /// <returns>شامل توکن دسترسی و توکن نوسازی جدید</returns>
        public async Task<TokenResultDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            // ۱. یافتن کاربر بر اساس توکن نوسازی
            var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);

            // ۲. بررسی وجود کاربر، تطابق شناسه و اعتبار توکن
            if (user is null || user.Id != dto.UserId || !user.IsRefreshTokenValid(dto.RefreshToken))
            {
                throw new UnauthorizedAccessException("توکن نوسازی نامعتبر است یا منقضی شده است.");
            }

            // ۳. تولید توکن‌های کاملاً جدید (فقط با استفاده از user، بدون کوئری اضافه برای Person)
            var newTokenResult = await _tokenService.GenerateTokensAsync(user);

            // ۴. اعمال چرخش توکن با استفاده از user.Id
            user.SetRefreshToken(newTokenResult.RefreshToken, newTokenResult.RefreshTokenExpiration, user.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            // ۵. بازگرداندن توکن‌های جدید به کلاینت
            return newTokenResult;
        }

        /// <summary>
        /// خروج از سیستم و باطل کردن توکن نوسازی جاری (Invalidate Refresh Token)
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">در صورت عدم احراز هویت کاربر</exception>
        /// <exception cref="NotFoundException">در صورت عدم یافت شدن کاربر</exception>
        public async Task LogoutAsync()
        {
            // ۱. بررسی احراز هویت کاربر جاری
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException("کاربر احراز هویت نشده است.");

            // ۲. یافتن کاربر بر اساس شناسه استخراج‌شده از توکن
            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
            if (user is null)
                throw new NotFoundException("کاربر یافت نشد.", DomainConcept.User, nameof(User));

            // ۳. باطل کردن توکن نوسازی در سمت سرور (Logout واقعی در الگوی JWT)
            user.RemoveRefreshToken(user.Id);

            // ۴. ذخیره تغییرات در دیتابیس
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        /// <summary>
        /// دریافت پروفایل کاربر جاری
        /// </summary>
        /// <returns>اطلاعات پروفایل کاربر</returns>
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