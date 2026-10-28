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
        private readonly IPasswordHasher _passwordHasher;

        /// <summary>
        /// سازنده سرویس احراز هویت و مقداردهی وابستگی‌ها
        /// </summary>
        public AuthService(
            IUserRepository userRepository,
            IPersonRepository personRepository,
            IMessageService messageService,
            ICurrentUserService currentUserService,
            ITokenService tokenService,
            IOtpCacheService otpCacheService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
            _messageService = messageService;
            _currentUserService = currentUserService;
            _tokenService = tokenService;
            _otpCacheService = otpCacheService;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// درخواست کد تأیید (OTP) و ذخیره آن در کش (بدون ایجاد رکورد اضافی در دیتابیس)
        /// </summary>
        public async Task RequestOtpAsync(RequestOtpDto dto)
        {
            var otpCode = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
            var expiration = TimeSpan.FromMinutes(2);

            await _otpCacheService.SetOtpAsync(dto.Mobile, otpCode, expiration);
            await _messageService.SendToMobileAsync(dto.Mobile, $"کد تأیید شما: {otpCode}\nاین کد تا ۲ دقیقه معتبر است.");
        }

        /// <summary>
        /// تأیید کد OTP، ساخت/یافتن کاربر و ورود به سیستم (همراه با محافظت در برابر Brute Force)
        /// </summary>
        public async Task<TokenResultDto> VerifyOtpAsync(VerifyOtpDto dto)
        {
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

            var cachedOtp = await _otpCacheService.GetOtpAsync(dto.Mobile);

            if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.Code)
            {
                if (existingUser != null)
                {
                    existingUser.IncrementAccessFailedCount(existingUser.Id);
                    _userRepository.Update(existingUser);
                    await _userRepository.SaveChangesAsync();
                }
                throw new UnauthorizedAccessException("کد تأیید نامعتبر یا منقضی شده است.");
            }

            if (person is null)
            {
                person = new Person(createBy: 0, mobile: dto.Mobile);
                await _personRepository.AddAsync(person);
                await _personRepository.SaveChangesAsync();
            }

            var user = await _userRepository.GetByPersonIdAsync(person.Id);
            if (user is null)
            {
                user = new User(createBy: person.Id, username: dto.Mobile, personId: person.Id);
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            await _otpCacheService.RemoveOtpAsync(dto.Mobile);

            user.ResetAccessFailedCount(user.Id);
            user.RecordLogin(ipAddress: "127.0.0.1", updateBy: user.Id);

            var tokenResult = await _tokenService.GenerateTokensAsync(user);
            user.SetRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiration, user.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return tokenResult;
        }

        /// <summary>
        /// درخواست کد تأیید (OTP) مخصوص کاربران دارای نقش سیستمی (ادمین)
        /// اگر کاربر نقش سیستمی نداشته باشد، پیام موفقیت کاذب نمایش داده می‌شود (جلوگیری از User Enumeration)
        /// </summary>
        public async Task RequestAdminOtpAsync(RequestOtpDto dto)
        {
            // دریافت کاربر همراه با نقش‌ها
            var user = await _userRepository.GetByMobileWithRolesAsync(dto.Mobile);

            // بررسی وجود کاربر و داشتن حداقل یک نقش سیستمی
            bool isAdmin = user?.HasSystemRole() ?? false;

            // همیشه پیام موفقیت نمایش می‌دهیم تا مهاجم نفهمد این شماره ادمین است یا خیر
            if (!isAdmin)
            {
                return; // خروج زودرس با پیام موفقیت کاذب در کنترلر
            }

            var otpCode = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
            var expiration = TimeSpan.FromMinutes(2);

            await _otpCacheService.SetOtpAsync(dto.Mobile, otpCode, expiration);
            await _messageService.SendToMobileAsync(dto.Mobile, $"کد تأیید پنل مدیریت: {otpCode}\nاین کد تا ۲ دقیقه معتبر است.");
        }

        /// <summary>
        /// تأیید کد OTP ادمین و صدور توکن موقت (StepToken)
        /// </summary>
        public async Task<StepTokenResultDto> VerifyAdminOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userRepository.GetByMobileWithRolesAsync(dto.Mobile);

            if (user == null || !user.HasSystemRole())
            {
                throw new UnauthorizedAccessException("کد تأیید نامعتبر یا منقضی شده است.");
            }

            var cachedOtp = await _otpCacheService.GetOtpAsync(dto.Mobile);

            if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.Code)
            {
                user.IncrementAccessFailedCount(user.Id);
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                throw new UnauthorizedAccessException("کد تأیید نامعتبر یا منقضی شده است.");
            }

            // پاک کردن OTP از کش پس از استفاده موفق
            await _otpCacheService.RemoveOtpAsync(dto.Mobile);

            // تولید توکن موقت ۵ دقیقه‌ای
            var stepToken = await _tokenService.GenerateStepTokenAsync(user);

            return new StepTokenResultDto
            {
                StepToken = stepToken,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            };
        }

        /// <summary>
        /// ورود نهایی ادمین با استفاده از رمز عبور و توکن موقت معتبر
        /// </summary>
        public async Task<TokenResultDto> AdminPasswordLoginAsync(AdminPasswordLoginDto dto)
        {
            // ۱. اعتبارسنجی توکن موقت (بررسی امضا، انقضا، هدف و تطابق موبایل)
            var userId = await _tokenService.ValidateStepTokenAsync(dto.StepToken, dto.Mobile);

            // ۲. دریافت کاربر
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.HasSystemRole())
            {
                throw new UnauthorizedAccessException("کاربر یافت نشد یا دسترسی لازم را ندارد.");
            }

            // ۳. بررسی قفل بودن حساب
            if (user.IsLockedOut())
            {
                throw new ConflictException("حساب کاربری شما به دلیل تلاش‌های ناموفق متعدد موقتاً قفل شده است.", DomainConcept.User, nameof(User));
            }

            // ۴. اعتبارسنجی رمز عبور
            if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                user.IncrementAccessFailedCount(user.Id);
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                throw new UnauthorizedAccessException("رمز عبور نامعتبر است.");
            }

            // ۵. ورود موفق: ریست کردن خطاها و ثبت لاگین
            user.ResetAccessFailedCount(user.Id);
            user.RecordLogin(ipAddress: "127.0.0.1", updateBy: user.Id);

            // ۶. تولید توکن‌های اصلی و ذخیره Refresh Token
            var tokenResult = await _tokenService.GenerateTokensAsync(user);
            user.SetRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiration, user.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return tokenResult;
        }

        /// <summary>
        /// نوسازی توکن دسترسی و توکن نوسازی با استفاده از الگوی چرخش توکن (Refresh Token Rotation)
        /// </summary>
        public async Task<TokenResultDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);

            if (user is null || user.Id != dto.UserId || !user.IsRefreshTokenValid(dto.RefreshToken))
            {
                throw new UnauthorizedAccessException("توکن نوسازی نامعتبر است یا منقضی شده است.");
            }

            var newTokenResult = await _tokenService.GenerateTokensAsync(user);
            user.SetRefreshToken(newTokenResult.RefreshToken, newTokenResult.RefreshTokenExpiration, user.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return newTokenResult;
        }

        /// <summary>
        /// خروج از سیستم و باطل کردن توکن نوسازی جاری (Invalidate Refresh Token)
        /// </summary>
        public async Task LogoutAsync()
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException("کاربر احراز هویت نشده است.");

            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
            if (user is null)
                throw new NotFoundException("کاربر یافت نشد.", DomainConcept.User, nameof(User));

            user.RemoveRefreshToken(user.Id);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        /// <summary>
        /// دریافت پروفایل کاربر جاری
        /// </summary>
        public async Task<UserProfileDto> GetCurrentProfileAsync()
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException("کاربر احراز هویت نشده است.");

            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
            if (user is null)
                throw new NotFoundException("کاربر یافت نشد.", DomainConcept.User, nameof(User));

            var person = await _personRepository.GetByIdAsync(user.PersonId);
            if (person is null)
                throw new NotFoundException("اطلاعات شخص یافت نشد.", DomainConcept.Person, nameof(Person));

            return person.ToDto();
        }
    }
}