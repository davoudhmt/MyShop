using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// موجودیت شخص (اطلاعات هویتی و تماس)
    /// این کلاس می‌تواند مستقل از User وجود داشته باشد (مثلاً مشتری که فقط با موبایل وارد شده است)
    /// </summary>
    public class Person : BaseEntity
    {
        /// <summary>
        /// نام شخص (اختیاری)
        /// </summary>
        public string? FirstName { get; private set; }

        /// <summary>
        /// نام خانوادگی شخص (اختیاری)
        /// </summary>
        public string? LastName { get; private set; }

        /// <summary>
        /// شماره موبایل (اجباری - برای احراز هویت و OTP)
        /// </summary>
        public MobileNumber Mobile { get; private set; } = null!;

        /// <summary>
        /// شماره تلفن ثابت (اختیاری - صرفاً برای اطلاعات تماس)
        /// </summary>
        public LandlineNumber? Landline { get; private set; }

        /// <summary>
        /// آدرس ایمیل (اختیاری - برای ارسال فاکتور)
        /// </summary>
        public Email? Email { get; private set; }

        /// <summary>
        /// شناسه حساب کاربری مرتبط (اختیاری)
        /// در صورت null بودن، یعنی این شخص هنوز حساب کاربری ندارد
        /// </summary>
        public long? UserId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به User (اختیاری)
        /// </summary>
        public User? User { get; private set; }

        /// <summary>
        /// نام کامل شخص (اگر نام و نام خانوادگی پر شده باشد)
        /// این خاصیت محاسباتی است و در دیتابیس ذخیره نمی‌شود
        /// </summary>
        public string? FullName => string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? null
            : $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// سازنده پیش‌فرض برای EF Core
        /// </summary>
        protected Person() { }

        /// <summary>
        /// سازنده برای ایجاد شخص جدید (فقط موبایل اجباری است)
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="mobile">شماره موبایل (اجباری)</param>
        /// <param name="firstName">نام شخص (اختیاری)</param>
        /// <param name="lastName">نام خانوادگی شخص (اختیاری)</param>
        /// <param name="landline">شماره تلفن ثابت (اختیاری)</param>
        /// <param name="email">آدرس ایمیل (اختیاری)</param>
        public Person(
            long createBy,
            string mobile,
            string? firstName = null,
            string? lastName = null,
            string? landline = null,
            string? email = null) : base(createBy)
        {
            InitializePerson(firstName, lastName, mobile, landline, email);
        }

        /// <summary>
        /// متد مقداردهی اولیه شخص با استفاده از متدهای اعتبارسنجی
        /// </summary>
        /// <param name="firstName">نام شخص (اختیاری)</param>
        /// <param name="lastName">نام خانوادگی شخص (اختیاری)</param>
        /// <param name="mobile">شماره موبایل (اجباری)</param>
        /// <param name="landline">شماره تلفن ثابت (اختیاری)</param>
        /// <param name="email">آدرس ایمیل (اختیاری)</param>
        private void InitializePerson(string? firstName, string? lastName, string mobile, string? landline, string? email)
        {
            SetFirstName(firstName);
            SetLastName(lastName);
            SetMobile(mobile);
            SetLandline(landline);
            SetEmail(email);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        /// <summary>
        /// تنظیم و اعتبارسنجی نام شخص
        /// در صورت خالی بودن مقدار، فیلد را null قرار می‌دهد (چون اختیاری است)
        /// </summary>
        /// <param name="firstName">نام ورودی (اختیاری)</param>
        /// <exception cref="DomainException">در صورت طول نامعتبر (وقتی مقدار خالی نیست)</exception>
        private void SetFirstName(string? firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                FirstName = null;
                return;
            }

            var normalized = firstName.Trim();
            if (Guard.IsInvalidLength(normalized, 2, 100))
                throw new DomainException("نام باید بین 2 تا 100 کاراکتر باشد.", DomainConcept.Person, nameof(FirstName));

            FirstName = normalized;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی نام خانوادگی شخص
        /// در صورت خالی بودن مقدار، فیلد را null قرار می‌دهد (چون اختیاری است)
        /// </summary>
        /// <param name="lastName">نام خانوادگی ورودی (اختیاری)</param>
        /// <exception cref="DomainException">در صورت طول نامعتبر (وقتی مقدار خالی نیست)</exception>
        private void SetLastName(string? lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                LastName = null;
                return;
            }

            var normalized = lastName.Trim();
            if (Guard.IsInvalidLength(normalized, 2, 100))
                throw new DomainException("نام خانوادگی باید بین 2 تا 100 کاراکتر باشد.", DomainConcept.Person, nameof(LastName));

            LastName = normalized;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی شماره موبایل
        /// </summary>
        /// <param name="mobile">شماره موبایل ورودی</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن فرمت</exception>
        private void SetMobile(string mobile)
        {
            Mobile = MobileNumber.Create(mobile);
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی شماره تلفن ثابت
        /// در صورت خالی بودن مقدار، فیلد را null قرار می‌دهد (چون اختیاری است)
        /// </summary>
        /// <param name="landline">شماره تلفن ثابت ورودی (اختیاری)</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن فرمت (وقتی مقدار خالی نیست)</exception>
        private void SetLandline(string? landline)
        {
            if (string.IsNullOrWhiteSpace(landline))
            {
                Landline = null;
                return;
            }

            Landline = LandlineNumber.Create(landline);
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی آدرس ایمیل
        /// در صورت خالی بودن مقدار، فیلد را null قرار می‌دهد (چون اختیاری است)
        /// </summary>
        /// <param name="email">آدرس ایمیل ورودی (اختیاری)</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن فرمت (وقتی مقدار خالی نیست)</exception>
        private void SetEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                Email = null;
                return;
            }

            Email = Email.Create(email);
        }

        // ===== متدهای به‌روزرسانی عمومی (هر فیلد متد جداگانه) =====

        /// <summary>
        /// به‌روزرسانی نام شخص
        /// </summary>
        /// <param name="firstName">نام جدید (اختیاری، در صورت خالی بودن حذف می‌شود)</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateFirstName(string? firstName, long updateBy)
        {
            SetFirstName(firstName);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// به‌روزرسانی نام خانوادگی شخص
        /// </summary>
        /// <param name="lastName">نام خانوادگی جدید (اختیاری، در صورت خالی بودن حذف می‌شود)</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateLastName(string? lastName, long updateBy)
        {
            SetLastName(lastName);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// به‌روزرسانی شماره موبایل
        /// </summary>
        /// <param name="mobile">شماره موبایل جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateMobile(string mobile, long updateBy)
        {
            SetMobile(mobile);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// به‌روزرسانی شماره تلفن ثابت
        /// </summary>
        /// <param name="landline">شماره تلفن ثابت جدید (اختیاری، در صورت خالی بودن حذف می‌شود)</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateLandline(string? landline, long updateBy)
        {
            SetLandline(landline);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// به‌روزرسانی آدرس ایمیل
        /// </summary>
        /// <param name="email">آدرس ایمیل جدید (اختیاری، در صورت خالی بودن حذف می‌شود)</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateEmail(string? email, long updateBy)
        {
            SetEmail(email);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// اتصال حساب کاربری به این شخص
        /// این متد در دو سناریو استفاده می‌شود:
        /// ۱. هنگام ثبت‌نام کاربر جدید
        /// ۲. هنگام تبدیل مشتری حضوری به کاربر سیستم (با وارد کردن شماره موبایل یکسان)
        /// </summary>
        /// <param name="userId">شناسه حساب کاربری</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن شناسه یا اتصال قبلی</exception>
        public void LinkUser(long userId, long updateBy)
        {
            if (Guard.IsInvalidId(userId))
                throw new DomainException("شناسه کاربری نامعتبر است.", DomainConcept.Person, nameof(UserId));

            if (UserId.HasValue)
                throw new DomainException("این شخص قبلاً به یک حساب کاربری متصل شده است.", DomainConcept.Person, nameof(UserId));

            UserId = userId;
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// قطع ارتباط حساب کاربری از این شخص (در صورت نیاز)
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UnlinkUser(long updateBy)
        {
            UserId = null;
            UpdateModificationInfo(updateBy);
        }
    }
}