using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای شماره موبایل
/// تضمین می‌کند که هر شماره موبایل ذخیره‌شده، فرمت معتبر ایران (09xxxxxxxxx) را دارد
/// این نوع داده برای احراز هویت (OTP) و ارسال پیامک استفاده می‌شود
/// </summary>
public class MobileNumber
{
    /// <summary>
    /// مقدار شماره موبایل
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// سازنده خصوصی برای اجبار استفاده از متد کارخانه (Factory Method)
    /// </summary>
    /// <param name="value">مقدار شماره موبایل</param>
    /// <exception cref="DomainException">در صورت نامعتبر بودن فرمت</exception>
    private MobileNumber(string value)
    {
        if (Guard.IsInvalidMobile(value))
            throw new DomainException("فرمت شماره موبایل نامعتبر است. فرمت صحیح: 09xxxxxxxxx", DomainConcept.Person, nameof(MobileNumber));

        Value = value;
    }

    /// <summary>
    /// متد کارخانه برای ساخت شیء MobileNumber با اعتبارسنجی
    /// </summary>
    /// <param name="value">مقدار شماره موبایل</param>
    /// <returns>شیء MobileNumber معتبر</returns>
    public static MobileNumber Create(string value) => new MobileNumber(value);

    /// <summary>
    /// تبدیل ضمنی از رشته به MobileNumber
    /// </summary>
    /// <param name="value">مقدار رشته‌ای</param>
    public static implicit operator MobileNumber(string value) => Create(value);

    /// <summary>
    /// تبدیل ضمنی از MobileNumber به رشته
    /// </summary>
    /// <param name="mobile">شیء MobileNumber</param>
    public static implicit operator string(MobileNumber mobile) => mobile?.Value ?? string.Empty;

    /// <summary>
    /// بررسی برابری دو شیء MobileNumber بر اساس مقدار
    /// </summary>
    /// <param name="obj">شیء مورد مقایسه</param>
    /// <returns>true در صورت برابری، در غیر این صورت false</returns>
    public override bool Equals(object? obj) => obj is MobileNumber other && Value == other.Value;

    /// <summary>
    /// محاسبه کد هش بر اساس مقدار شماره موبایل
    /// </summary>
    /// <returns>کد هش</returns>
    public override int GetHashCode() => Value.GetHashCode();
}