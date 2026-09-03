using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object برای تنظیمات سئوی محصول (همه با هم تغییر می‌کنن)
/// </summary>
public sealed class Seo : ValueObject
{
    /// <summary>عنوان سئو (نمایشی در نتایج گوگل)</summary>
    public string? Title { get; }

    /// <summary>تگ H1 صفحه</summary>
    public string? H1 { get; }

    /// <summary>توضیح سئو</summary>
    public string? Description { get; }

    /// <summary>تگ meta description</summary>
    public string? MetaDescription { get; }

    /// <summary>تگ meta keywords</summary>
    public string? MetaKeywords { get; }

    public Seo(
        string? title = null,
        string? h1 = null,
        string? description = null,
        string? metaDescription = null,
        string? metaKeywords = null)
    {
        Title = Validate(title, nameof(Title), 200);
        H1 = Validate(h1, nameof(H1), 200);
        Description = Validate(description, nameof(Description), 500);
        MetaDescription = Validate(metaDescription, nameof(MetaDescription), 300);
        MetaKeywords = Validate(metaKeywords, nameof(MetaKeywords), 300);
    }

    /// <summary>
    /// اعتبارسنجی و نرمال‌سازی هر فیلد سئو
    /// </summary>
    private static string? Validate(string? value, string fieldName, int maxLength)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        if (Guard.IsOverMaxLength(normalized, maxLength))
            throw new DomainException($"فیلد سئو {fieldName} نمی‌تواند بیش از {maxLength} کاراکتر باشد.", DomainConcept.Seo, fieldName);

        return normalized;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Title;
        yield return H1;
        yield return Description;
        yield return MetaDescription;
        yield return MetaKeywords;
    }
}