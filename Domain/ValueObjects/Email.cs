using System.Text.RegularExpressions;
using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای آدرس ایمیل
/// تضمین می‌کند که هر ایمیل ذخیره‌شده، فرمت معتبر دارد
/// این نوع داده صرفاً برای اطلاعات تماس و ارسال فاکتور استفاده می‌شود
/// </summary>
public class Email
{
    /// <summary>
    /// مقدار آدرس ایمیل
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// سازنده خصوصی برای اجبار استفاده از متد کارخانه (Factory Method)
    /// </summary>
    /// <param name="value">مقدار آدرس ایمیل</param>
    /// <exception cref="DomainException">در صورت نامعتبر بودن فرمت</exception>
    private Email(string value)
    {
        if (Guard.IsInvalidEmail(value))
            throw new DomainException("فرمت آدرس ایمیل نامعتبر است.", DomainConcept.Person, nameof(Email));

        Value = value.ToLower().Trim();
    }

    /// <summary>
    /// متد کارخانه برای ساخت شیء Email با اعتبارسنجی
    /// </summary>
    /// <param name="value">مقدار آدرس ایمیل</param>
    /// <returns>شیء Email معتبر</returns>
    public static Email Create(string value) => new Email(value);

    /// <summary>
    /// تبدیل ضمنی از رشته به Email
    /// </summary>
    /// <param name="value">مقدار رشته‌ای</param>
    public static implicit operator Email(string value) => Create(value);

    /// <summary>
    /// تبدیل ضمنی از Email به رشته
    /// </summary>
    /// <param name="email">شیء Email</param>
    public static implicit operator string(Email email) => email?.Value ?? string.Empty;

    /// <summary>
    /// بررسی برابری دو شیء Email بر اساس مقدار
    /// </summary>
    /// <param name="obj">شیء مورد مقایسه</param>
    /// <returns>true در صورت برابری، در غیر این صورت false</returns>
    public override bool Equals(object? obj) => obj is Email other && Value == other.Value;

    /// <summary>
    /// محاسبه کد هش بر اساس مقدار ایمیل
    /// </summary>
    /// <returns>کد هش</returns>
    public override int GetHashCode() => Value.GetHashCode();
}