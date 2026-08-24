using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای آدرس تصویر (URL نسبی یا مطلق)
/// </summary>
public sealed class ImageUrl : ValueObject
{
    public string? Value { get; }

    public ImageUrl(string? value)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        if (normalized is not null)
        {
            var validUrl = normalized;

            if (Guard.IsInvalidUrl(validUrl))
                throw new DomainException("آدرس تصویر معتبر نمی‌باشد.", DomainConcept.ImageUrl, nameof(Value));

            if (Guard.IsInvalidImageExtension(validUrl))
                throw new DomainException("نوع تصویر معتبر نمی‌باشد.", DomainConcept.ImageUrl, nameof(Value));
        }

        Value = normalized;
    }
    /// <summary>
    /// تلاش برای ساخت ImageUrl بدون پرتاب استثنا
    /// </summary>
    /// <param name="value"></param>
    /// <param name="imageUrl"></param>
    /// <returns></returns>
    public static bool TryCreate(string? value, out ImageUrl? imageUrl)
    {
        imageUrl = null;

        var normalized = string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        if (normalized is not null)
        {
            var validUrl = normalized;

            if (Guard.IsInvalidUrl(validUrl))
                return false;

            if (Guard.IsInvalidImageExtension(validUrl))
                return false;
        }

        imageUrl = new ImageUrl(normalized);
        return true;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator string?(ImageUrl? imageUrl) => imageUrl?.Value;
    public static explicit operator ImageUrl?(string? value) => new(value);
}