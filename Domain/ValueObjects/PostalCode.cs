using System.Text.RegularExpressions;
using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای کد پستی
/// تضمین می‌کند که هر کد پستی ذخیره‌شده، فرمت معتبر ۱۰ رقمی دارد
/// </summary>
public class PostalCode
{
    /// <summary>
    /// Regex برای اعتبارسنجی فرمت کد پستی ایران (۱۰ رقم)
    /// </summary>
    private static readonly Regex PostalCodeRegex = new Regex(@"^\d{10}$", RegexOptions.Compiled);

    /// <summary>
    /// مقدار کد پستی
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// سازنده خصوصی برای اجبار استفاده از متد کارخانه (Factory Method)
    /// </summary>
    /// <param name="value">مقدار کد پستی</param>
    /// <exception cref="DomainException">در صورت نامعتبر بودن فرمت</exception>
    private PostalCode(string value)
    {
        if (Guard.IsInvalidPostalCode(value))
            throw new DomainException("فرمت کد پستی نامعتبر است. کد پستی باید ۱۰ رقم باشد.", DomainConcept.Address, nameof(PostalCode));

        Value = value;
    }

    /// <summary>
    /// متد کارخانه برای ساخت شیء PostalCode با اعتبارسنجی
    /// </summary>
    /// <param name="value">مقدار کد پستی</param>
    /// <returns>شیء PostalCode معتبر</returns>
    public static PostalCode Create(string value) => new PostalCode(value);

    /// <summary>
    /// تبدیل ضمنی از رشته به PostalCode
    /// </summary>
    /// <param name="value">مقدار رشته‌ای</param>
    public static implicit operator PostalCode(string value) => Create(value);

    /// <summary>
    /// تبدیل ضمنی از PostalCode به رشته
    /// </summary>
    /// <param name="postalCode">شیء PostalCode</param>
    public static implicit operator string(PostalCode postalCode) => postalCode?.Value ?? string.Empty;

    /// <summary>
    /// بررسی برابری دو شیء PostalCode بر اساس مقدار
    /// </summary>
    /// <param name="obj">شیء مورد مقایسه</param>
    /// <returns>true در صورت برابری، در غیر این صورت false</returns>
    public override bool Equals(object? obj) => obj is PostalCode other && Value == other.Value;

    /// <summary>
    /// محاسبه کد هش بر اساس مقدار کد پستی
    /// </summary>
    /// <returns>کد هش</returns>
    public override int GetHashCode() => Value.GetHashCode();
}