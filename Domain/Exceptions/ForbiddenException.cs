namespace Domain.Exceptions;

/// <summary>
/// استثنا برای زمانی که کاربر مجوز انجام عملیات را ندارد (منجر به 403 می‌شود)
/// </summary>
public class ForbiddenException : DomainException
{
    /// <summary>
    /// ایجاد یک استثنا برای زمانی که کاربر مجوز انجام عملیات را ندارد (منجر به 403 می‌شود)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="domainConcept"></param>
    /// <param name="propertyName"></param>
    public ForbiddenException(string message, string domainConcept, string propertyName)
        : base(message, domainConcept, propertyName, null) { }
}