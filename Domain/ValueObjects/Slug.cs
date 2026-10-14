using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای آدرس URL محصول (مانند: samsung-galaxy-s24)
/// </summary>
public sealed class Slug : ValueObject
{
    public string Value { get; }

    /// <summary>
    /// مقداردهی اولیه برای اسلاگ محصول
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="DomainException"></exception>
    public Slug(string value)
    {
        var normalized = value?.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new DomainException("اسلاگ نمی‌تواند خالی باشد.", DomainConcept.Slug, nameof(Value));

        if (Guard.IsOverMaxLength(normalized, 200))
            throw new DomainException("اسلاگ نمی‌تواند بیش از 200 کاراکتر باشد.", DomainConcept.Slug, nameof(Value));

        if (!IsValidFormat(normalized))
            throw new DomainException("اسلاگ فقط می‌تواند شامل حروف کوچک، اعداد و خط تیره باشد.", DomainConcept.Slug, nameof(Value));

        Value = normalized;
    }

    /// <summary>
    /// تلاش برای ساخت Slug بدون پرتاب استثنا
    /// </summary>
    public static bool TryCreate(string? value, out Slug? slug)
    {
        slug = null;
        var normalized = value?.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalized) || Guard.IsOverMaxLength(normalized, 200) || !IsValidFormat(normalized))
            return false;

        slug = new Slug(normalized);
        return true;
    }

    /// <summary>
    /// فقط حروف کوچک، اعداد و خط تیره - بدون خط تیره در ابتدا/انتها یا پشت سر هم
    /// </summary>
    private static bool IsValidFormat(string value)
        => value.All(c => char.IsAsciiLetterOrDigit(c) || c == '-')
           && !value.StartsWith('-')
           && !value.EndsWith('-')
           && !value.Contains("--");

    /// <summary>
    /// اجزای برابری برای مقایسه Value Object
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// بازگرداندن مقدار اسلاگ به صورت رشته
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value;

    /// <summary>
    /// تبدیل ضمنی Slug به رشته
    /// </summary>
    /// <param name="slug"></param>
    public static implicit operator string(Slug slug) => slug.Value;

    /// <summary>
    /// تبدیل صریح رشته به Slug
    /// </summary>
    /// <param name="value"></param>
    public static explicit operator Slug(string value) => new(value);
}