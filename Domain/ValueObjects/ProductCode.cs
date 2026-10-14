using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای کد محصول با فرمت: 3 حرف + 3 رقم
/// </summary>
public sealed class ProductCode : ValueObject
{
    public string Value { get; }

    /// <summary>
    /// سازنده ProductCode با اعتبارسنجی فرمت و طول
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="DomainException"></exception>
    public ProductCode(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (Guard.IsInvalidLength(normalized, 6, 6))
            throw new DomainException("کد محصول باید دقیقاً 6 کاراکتر باشد.", DomainConcept.ProductCode, nameof(Value));

        var validValue = normalized!;

        if (!IsValidFormat(validValue))
            throw new DomainException("کد محصول باید دقیقاً شامل 3 حرف و 3 رقم باشد.", DomainConcept.ProductCode, nameof(Value));

        Value = validValue;
    }

    /// <summary>
    /// تلاش برای ساخت ProductCode بدون پرتاب استثنا
    /// </summary>
    /// <param name="value"></param>
    /// <param name="productCode"></param>
    /// <returns></returns>
    public static bool TryCreate(string? value, out ProductCode? productCode)
    {
        productCode = null;

        var normalized = value?.Trim().ToUpperInvariant();

        if (Guard.IsInvalidLength(normalized, 6, 6))
            return false;

        var validValue = normalized!;

        if (!IsValidFormat(validValue))
            return false;

        productCode = new ProductCode(validValue);
        return true;
    }

    /// <summary>
    /// اعتبارسنجی فرمت کد محصول: 3 حرف + 3 رقم
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsValidFormat(string value)
        => value[..3].All(char.IsAsciiLetter) && value[3..].All(char.IsAsciiDigit);

    /// <summary>
    /// برگرداندن اجزای برابری برای مقایسه Value Object
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// برگرداندن رشته نمایشی از کد محصول
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value;

    /// <summary>
    /// تبدیل ضمنی ProductCode به رشته
    /// </summary>
    /// <param name="productCode"></param>
    public static implicit operator string(ProductCode productCode) => productCode.Value;

    /// <summary>
    /// تبدیل صریح رشته به ProductCode
    /// </summary>
    /// <param name="value"></param>
    public static explicit operator ProductCode(string value) => new(value);
}