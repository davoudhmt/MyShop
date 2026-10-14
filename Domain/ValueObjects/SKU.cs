using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای کد یکتای محصول با فرمت: 3 حرف + 7 عدد
/// </summary>
public sealed class SKU : ValueObject
{
    public string Value { get; }

    /// <summary>
    /// مقداردهی اولیه برای کد یکتا (SKU) محصول
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="DomainException"></exception>
    public SKU(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant();

        if (Guard.IsInvalidLength(normalized, 10, 10))
            throw new DomainException("کد یکتا باید دقیقاً 10 کاراکتر باشد.", DomainConcept.SKU, nameof(Value));

        var validValue = normalized!;

        if (!IsValidFormat(validValue))
            throw new DomainException("کد یکتا (SKU) باید شامل 3 حرف و 7 عدد باشد.", DomainConcept.SKU, nameof(Value));

        Value = validValue;
    }

    /// <summary>
    ///  تلاش برای ساخت SKU بدون پرتاب استثنا
    /// </summary>
    /// <param name="value"></param>
    /// <param name="sku"></param>
    /// <returns></returns>
    public static bool TryCreate(string? value, out SKU? sku)
    {
        sku = null;

        var normalized = value?.Trim().ToUpperInvariant();

        if (Guard.IsInvalidLength(normalized, 10, 10))
            return false;

        var validValue = normalized!;

        if (!IsValidFormat(validValue))
            return false;

        sku = new SKU(validValue);
        return true;
    }

    /// <summary>
    /// اعتبارسنجی فرمت کد یکتا (SKU) محصول
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsValidFormat(string value)
        => value[..3].All(char.IsAsciiLetter) && value[3..].All(char.IsAsciiDigit);

    /// <summary>
    /// بررسی برابری دو شیء SKU با استفاده از مقادیر داخلی آن‌ها
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// بازگرداندن رشته نمایشی از کد یکتا (SKU) محصول
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value;

    /// <summary>
    /// تبدیل ضمنی از SKU به رشته
    /// </summary>
    /// <param name="sku"></param>
    public static implicit operator string(SKU sku) => sku.Value;

    /// <summary>
    /// تبدیل صریح از رشته به SKU
    /// </summary>
    /// <param name="value"></param>
    public static explicit operator SKU(string value) => new(value);
}