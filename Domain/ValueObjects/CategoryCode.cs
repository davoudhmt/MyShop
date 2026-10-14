using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای کد دسته‌بندی محصول: دقیقاً 3 حرف
/// </summary>
public sealed class CategoryCode : ValueObject
{
    public string Value { get; }

    /// <summary>
    /// ایجاد یک CategoryCode جدید با اعتبارسنجی
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="DomainException"></exception>
    public CategoryCode(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (Guard.IsInvalidLength(normalized, 3, 3))
            throw new DomainException("کد دسته‌بندی باید دقیقاً 3 کاراکتر باشد.", DomainConcept.CategoryCode, nameof(Value));

        var validValue = normalized!;

        if (!validValue.All(char.IsAsciiLetter))
            throw new DomainException("کد دسته‌بندی باید فقط شامل حروف باشد.", DomainConcept.CategoryCode, nameof(Value));

        Value = validValue;
    }

    /// <summary>
    ///  تلاش برای ساخت CategoryCode بدون پرتاب استثنا
    /// </summary>
    /// <param name="value"></param>
    /// <param name="categoryCode"></param>
    /// <returns></returns>
    public static bool TryCreate(string? value, out CategoryCode? categoryCode)
    {
        categoryCode = null;

        var normalized = value?.Trim().ToUpperInvariant();

        if (Guard.IsInvalidLength(normalized, 3, 3))
            return false;

        var validValue = normalized!;

        if (!validValue.All(char.IsAsciiLetter))
            return false;

        categoryCode = new CategoryCode(validValue);
        return true;
    }

    /// <summary>
    /// برگرداندن اجزای برابری برای مقایسه Value Object
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// برگرداندن رشته نمایشی از Value Object
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value;

    /// <summary>
    /// تبدیل ضمنی CategoryCode به رشته
    /// </summary>
    /// <param name="categoryCode"></param>
    public static implicit operator string(CategoryCode categoryCode) => categoryCode.Value;

    /// <summary>
    /// تبدیل صریح رشته به CategoryCode
    /// </summary>
    /// <param name="value"></param>
    public static explicit operator CategoryCode(string value) => new(value);
}