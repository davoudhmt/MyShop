using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای شماره تلفن ثابت
/// تضمین می‌کند که شماره تلفن ثابت (در صورت وجود) فرمت معتبر ایران را دارد
/// این نوع داده صرفاً برای اطلاعات تماس استفاده می‌شود و در احراز هویت نقشی ندارد
/// </summary>
public class LandlineNumber
{
    /// <summary>
    /// مقدار شماره تلفن ثابت
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// سازنده خصوصی برای اجبار استفاده از متد کارخانه (Factory Method)
    /// </summary>
    /// <param name="value">مقدار شماره تلفن ثابت</param>
    /// <exception cref="DomainException">در صورت نامعتبر بودن فرمت</exception>
    private LandlineNumber(string value)
    {
        if (Guard.IsInvalidLandline(value))
            throw new DomainException("فرمت شماره تلفن ثابت نامعتبر است.", DomainConcept.Person, nameof(LandlineNumber));

        Value = value.ToLower().Trim();
    }

    /// <summary>
    /// متد کارخانه برای ساخت شیء LandlineNumber با اعتبارسنجی
    /// </summary>
    /// <param name="value">مقدار شماره تلفن ثابت</param>
    /// <returns>شیء LandlineNumber معتبر</returns>
    public static LandlineNumber Create(string value) => new LandlineNumber(value);

    /// <summary>
    /// تبدیل ضمنی از رشته به LandlineNumber
    /// </summary>
    /// <param name="value">مقدار رشته‌ای</param>
    public static implicit operator LandlineNumber(string value) => Create(value);

    /// <summary>
    /// تبدیل ضمنی از LandlineNumber به رشته
    /// </summary>
    /// <param name="landline">شیء LandlineNumber</param>
    public static implicit operator string(LandlineNumber landline) => landline?.Value ?? string.Empty;

    /// <summary>
    /// بررسی برابری دو شیء LandlineNumber بر اساس مقدار
    /// </summary>
    /// <param name="obj">شیء مورد مقایسه</param>
    /// <returns>true در صورت برابری، در غیر این صورت false</returns>
    public override bool Equals(object? obj) => obj is LandlineNumber other && Value == other.Value;

    /// <summary>
    /// محاسبه کد هش بر اساس مقدار شماره تلفن ثابت
    /// </summary>
    /// <returns>کد هش</returns>
    public override int GetHashCode() => Value.GetHashCode();
}