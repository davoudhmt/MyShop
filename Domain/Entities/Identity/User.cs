using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// موجودیت کاربر سیستم (مسئول احراز هویت)
    /// این کلاس فقط اطلاعات مربوط به حساب کاربری را نگه می‌دارد
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
        /// تعداد تلاش‌های ناموفق ورود (برای محافظت در برابر Brute Force)
        /// </summary>
        public int AccessFailedCount { get; private set; } = 0;

        /// <summary>
        /// زمان پایان قفل شدن حساب (در صورت تلاش‌های ناموفق زیاد)
        /// </summary>
        public DateTime? LockoutEnd { get; private set; }

        /// <summary>
        /// کد امنیتی برای invalidation توکن‌ها (هر بار رمز عوض می‌شه، تغییر می‌کنه)
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
        /// <param name="username">نام کاربری</param>
        /// <param name="personId">شناسه Person</param>
        private void InitializeUser(string username, long personId)
        {
            SetUsername(username);
            SetPersonId(personId);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        /// <summary>
        /// تنظیم و اعتبارسنجی نام کاربری
        /// </summary>
        /// <param name="username">نام کاربری ورودی</param>
        /// <exception cref="DomainException">در صورت خالی بودن یا نامعتبر بودن فرمت</exception>
        private void SetUsername(string username)
        {
            var normalized = username?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("نام کاربری نمی‌تواند خالی باشد.", DomainConcept.User, nameof(Username));

            if (Guard.IsInvalidMobile(normalized))
                throw new DomainException("نام کاربری باید فرمت شماره موبایل معتبر داشته باشد.", DomainConcept.User, nameof(Username));

            Username = normalized;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی شناسه Person
        /// </summary>
        /// <param name="personId">شناسه Person</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن</exception>
        private void SetPersonId(long personId)
        {
            if (Guard.IsInvalidId(personId))
                throw new DomainException("شناسه شخص نامعتبر است.", DomainConcept.User, nameof(PersonId));

            PersonId = personId;
        }

        // ===== متدهای به‌روزرسانی عمومی =====

        /// <summary>
        /// به‌روزرسانی نام کاربری
        /// </summary>
        /// <param name="username">نام کاربری جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateUsername(string username, long updateBy)
        {
            SetUsername(username);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// تنظیم رمز عبور (هش شده)
        /// </summary>
        /// <param name="passwordHash">هش رمز عبور</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        /// <exception cref="DomainException">در صورت خالی بودن</exception>
        public void SetPasswordHash(string passwordHash, long updateBy)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("هش رمز عبور نمی‌تواند خالی باشد.", DomainConcept.User, nameof(PasswordHash));

            PasswordHash = passwordHash;
            SecurityStamp = Guid.NewGuid().ToString(); // تغییر SecurityStamp برای invalidation توکن‌ها
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// حذف رمز عبور (برای مشتریانی که فقط با OTP وارد می‌شوند)
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void RemovePassword(long updateBy)
        {
            PasswordHash = null;
            SecurityStamp = Guid.NewGuid().ToString();
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// ثبت تلاش ناموفق ورود
        /// اگر 3 بار تلاش ناموفق، حساب قفل می‌شود
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void IncrementAccessFailedCount(long updateBy)
        {
            AccessFailedCount++;

            if (AccessFailedCount >= 3)
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }

            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// ریست کردن شمارنده تلاش‌های ناموفق (پس از ورود موفق)
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void ResetAccessFailedCount(long updateBy)
        {
            AccessFailedCount = 0;
            LockoutEnd = null;
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// بررسی آیا حساب قفل است
        /// </summary>
        /// <returns>true در صورت قفل بودن، false در غیر این صورت</returns>
        public bool IsLockedOut()
        {
            return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
        }

        /// <summary>
        /// فعال‌سازی احراز هویت دو مرحله‌ای (2FA)
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void EnableTwoFactor(long updateBy)
        {
            TwoFactorEnabled = true;
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// غیرفعال‌سازی احراز هویت دو مرحله‌ای (2FA)
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void DisableTwoFactor(long updateBy)
        {
            TwoFactorEnabled = false;
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// ثبت اطلاعات ورود موفق
        /// </summary>
        /// <param name="ipAddress">آدرس IP کاربر</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void RecordLogin(string? ipAddress, long updateBy)
        {
            LastLoginDate = DateTime.UtcNow;
            LastLoginIp = ipAddress;
            ResetAccessFailedCount(updateBy);
        }

        /// <summary>
        /// تنظیم Refresh Token
        /// </summary>
        /// <param name="refreshToken">توکن نوسازی</param>
        /// <param name="expiryTime">زمان انقضای توکن</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        /// <exception cref="DomainException">در صورت خالی بودن توکن</exception>
        public void SetRefreshToken(string refreshToken, DateTime expiryTime, long updateBy)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new DomainException("Refresh Token نمی‌تواند خالی باشد.", DomainConcept.User, nameof(RefreshToken));

            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// حذف Refresh Token (برای Logout)
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void RemoveRefreshToken(long updateBy)
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// بررسی آیا Refresh Token معتبر است
        /// </summary>
        /// <param name="token">توکن مورد بررسی</param>
        /// <returns>true در صورت معتبر بودن، false در غیر این صورت</returns>
        public bool IsRefreshTokenValid(string token)
        {
            return !string.IsNullOrEmpty(RefreshToken)
                && RefreshToken == token
                && RefreshTokenExpiryTime.HasValue
                && RefreshTokenExpiryTime.Value > DateTime.UtcNow;
        }
    }
}