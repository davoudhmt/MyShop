using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای کد یکتای محصول با فرمت: 3 حرف + 7 عدد
/// </summary>
public sealed class SKU : ValueObject
{
    public string Value { get; }

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

    private static bool IsValidFormat(string value)
        => value[..3].All(char.IsAsciiLetter) && value[3..].All(char.IsAsciiDigit);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(SKU sku) => sku.Value;
    public static explicit operator SKU(string value) => new(value);
}