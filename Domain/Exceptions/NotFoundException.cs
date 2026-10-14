namespace Domain.Exceptions;

/// <summary>
/// استثنا برای زمانی که یک موجودیت در دامنه پیدا نمی‌شود (منجر به 404 می‌شود)
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// ایجاد یک استثنا برای زمانی که یک موجودیت در دامنه پیدا نمی‌شود (منجر به 404 می‌شود)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="domainConcept"></param>
    /// <param name="propertyName"></param>
    public NotFoundException(string message, string domainConcept, string propertyName)
        : base(message, domainConcept, propertyName, null) { }
}