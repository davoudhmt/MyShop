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

    private static bool IsValidFormat(string value)
        => value[..3].All(char.IsAsciiLetter) && value[3..].All(char.IsAsciiDigit);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ProductCode productCode) => productCode.Value;
    public static explicit operator ProductCode(string value) => new(value);
}