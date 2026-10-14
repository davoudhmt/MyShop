namespace Domain.Exceptions;

/// <summary>
/// استثنا برای زمانی که یک عملیات باعث تداخل می‌شود، مثل رکورد تکراری (منجر به 409 می‌شود)
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message, string domainConcept, string propertyName)
        : base(message, domainConcept, propertyName, null) { }
}