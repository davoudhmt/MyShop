using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// موجودیت کاربر سیستم (مسئول احراز هویت)
    /// این کلاس فقط اطلاعات مربوط به حساب کاربری و ورود را نگه می‌دارد
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// نام کاربری یونیک (فعلاً همان شماره موبایل)
        /// </summary>
        public string Username { get; private set; } = string.Empty;

        /// <summary>
        /// هش رمز عبور (Nullable چون مشتری ممکن است فقط با OTP وارد شود)
        /// </summary>
        public string? PasswordHash { get; private set; }

        /// <summary>
        /// کد تأیید یک‌بار مصرف (OTP) برای ورود
        /// </summary>
        public string? OtpCode { get; private set; }

        /// <summary>
        /// زمان انقضای کد تأیید یک‌بار مصرف (OTP)
        /// </summary>
        public DateTime? OtpExpiryTime { get; private set; }

        /// <summary>
        /// تعداد تلاش‌های ناموفق ورود (برای محافظت در برابر Brute Force)
        /// </summary>
        public int AccessFailedCount { get; private set; } = 0;

        /// <summary>
        /// زمان پایان قفل شدن حساب (در صورت تلاش‌های ناموفق زیاد)
        /// </summary>
        public DateTime? LockoutEnd { get; private set; }

        /// <summary>
        /// کد امنیتی برای invalidation توکن‌ها (هر بار رمز یا موبایل عوض می‌شه، تغییر می‌کنه)
        /// </summary>
        public string SecurityStamp { get; private set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// کد همزمانی برای جلوگیری از Race Condition
        /// </summary>
        public string ConcurrencyStamp { get; private set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// آیا احراز هویت دو مرحله‌ای (2FA) برای این کاربر فعال است؟
        /// </summary>
        public bool TwoFactorEnabled { get; private set; } = false;

        /// <summary>
        /// تاریخ و زمان آخرین ورود موفق
        /// </summary>
        public DateTime? LastLoginDate { get; private set; }

        /// <summary>
        /// آدرس IP آخرین ورود
        /// </summary>
        public string? LastLoginIp { get; private set; }

        /// <summary>
        /// توکن نوسازی برای JWT
        /// </summary>
        public string? RefreshToken { get; private set; }

        /// <summary>
        /// زمان انقضای Refresh Token
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        /// <summary>
        /// کلید خارجی برای ارتباط با Person
        /// </summary>
        public long PersonId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به Person
        /// </summary>
        public Person Person { get; private set; } = null!;

        /// <summary>
        /// لیست داخلی نقش‌های کاربر
        /// </summary>
        private readonly List<UserRole> _userRoles = new();

        /// <summary>
        /// نقش‌های تخصیص داده شده به این کاربر (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض برای EF Core
        /// </summary>
        protected User() { }

        /// <summary>
        /// سازنده برای ساخت کاربر جدید
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="username">نام کاربری (شماره موبایل)</param>
        /// <param name="personId">شناسه Person مرتبط</param>
        public User(long createBy, string username, long personId)
            : base(createBy)
        {
            InitializeUser(username, personId);
        }

        /// <summary>
        /// متد مقداردهی اولیه کاربر
        /// </summary>
        private void InitializeUser(string username, long personId)
        {
            SetUsername(username);
            SetPersonId(personId);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        private void SetUsername(string username)
        {
            var normalized = username?.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("نام کاربری نمی‌تواند خالی باشد.", DomainConcept.User, nameof(Username));

            if (Guard.IsInvalidMobile(normalized))
                throw new DomainException("نام کاربری باید فرمت شماره موبایل معتبر داشته باشد.", DomainConcept.User, nameof(Username));

            Username = normalized;
        }

        private void SetPersonId(long personId)
        {
            if (Guard.IsInvalidId(personId))
                throw new DomainException("شناسه شخص نامعتبر است.", DomainConcept.User, nameof(PersonId));

            PersonId = personId;
        }

        // ===== متدهای به‌روزرسانی عمومی =====

        public void UpdateUsername(string username, long updateBy)
        {
            SetUsername(username);
            UpdateModificationInfo(updateBy);
        }

        public void SetPasswordHash(string passwordHash, long updateBy)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("هش رمز عبور نمی‌تواند خالی باشد.", DomainConcept.User, nameof(PasswordHash));

            PasswordHash = passwordHash;
            SecurityStamp = Guid.NewGuid().ToString();
            UpdateModificationInfo(updateBy);
        }

        public void RemovePassword(long updateBy)
        {
            PasswordHash = null;
            SecurityStamp = Guid.NewGuid().ToString();
            UpdateModificationInfo(updateBy);
        }

        // ===== متدهای مدیریت OTP =====

        /// <summary>
        /// تولید و تنظیم کد تأیید (OTP) جدید
        /// </summary>
        /// <param name="code">کد تأیید تولید شده</param>
        /// <param name="expiryTime">زمان انقضای کد</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void GenerateOtp(string code, DateTime expiryTime, long updateBy)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("کد تأیید نمی‌تواند خالی باشد.", DomainConcept.User, nameof(OtpCode));

            OtpCode = code.Trim();
            OtpExpiryTime = expiryTime;
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// بررسی صحت کد تأیید وارد شده و پاک‌سازی آن در صورت موفقیت
        /// </summary>
        /// <param name="inputCode">کد وارد شده توسط کاربر</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        /// <exception cref="DomainException">در صورت منقضی شدن یا اشتباه بودن کد</exception>
        public void VerifyAndClearOtp(string inputCode, long updateBy)
        {
            if (string.IsNullOrWhiteSpace(OtpCode) || !OtpExpiryTime.HasValue || OtpExpiryTime.Value < DateTime.UtcNow)
            {
                throw new DomainException("کد تأیید نامعتبر یا منقضی شده است. لطفاً درخواست کد جدید دهید.", DomainConcept.User, nameof(OtpCode));
            }

            if (OtpCode != inputCode.Trim())
            {
                throw new DomainException("کد تأیید وارد شده اشتباه است.", DomainConcept.User, nameof(OtpCode));
            }

            // در صورت موفقیت، کد را پاک می‌کنیم تا قابل استفاده‌ی مجدد نباشد (جلوگیری از Replay Attack)
            OtpCode = null;
            OtpExpiryTime = null;
            UpdateModificationInfo(updateBy);
        }

        // ===== متدهای امنیتی و ورود =====

        public void IncrementAccessFailedCount(long updateBy)
        {
            AccessFailedCount++;
            if (AccessFailedCount >= 3)
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }
            UpdateModificationInfo(updateBy);
        }

        public void ResetAccessFailedCount(long updateBy)
        {
            AccessFailedCount = 0;
            LockoutEnd = null;
            UpdateModificationInfo(updateBy);
        }

        public bool IsLockedOut()
        {
            return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
        }

        public void EnableTwoFactor(long updateBy)
        {
            TwoFactorEnabled = true;
            UpdateModificationInfo(updateBy);
        }

        public void DisableTwoFactor(long updateBy)
        {
            TwoFactorEnabled = false;
            UpdateModificationInfo(updateBy);
        }

        public void RecordLogin(string? ipAddress, long updateBy)
        {
            LastLoginDate = DateTime.UtcNow;
            LastLoginIp = ipAddress;
            ResetAccessFailedCount(updateBy);
        }

        public void SetRefreshToken(string refreshToken, DateTime expiryTime, long updateBy)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new DomainException("Refresh Token نمی‌تواند خالی باشد.", DomainConcept.User, nameof(RefreshToken));

            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
            UpdateModificationInfo(updateBy);
        }

        public void RemoveRefreshToken(long updateBy)
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
            UpdateModificationInfo(updateBy);
        }

        public bool IsRefreshTokenValid(string token)
        {
            return !string.IsNullOrEmpty(RefreshToken)
                && RefreshToken == token
                && RefreshTokenExpiryTime.HasValue
                && RefreshTokenExpiryTime.Value > DateTime.UtcNow;
        }
    }
}