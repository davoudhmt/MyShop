namespace Domain.Exceptions;

/// <summary>
/// استثنا برای زمانی که یک موجودیت در دامنه پیدا نمی‌شود (منجر به 404 می‌شود)
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message, string domainConcept, string propertyName)
        : base(message, domainConcept, propertyName, null) { }
}