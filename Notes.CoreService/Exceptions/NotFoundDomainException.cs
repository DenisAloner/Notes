using System.Net;

namespace Notes.CoreService.Exceptions;

public class NotFoundDomainException<T> : DomainException
{
    public readonly T Key;

    public NotFoundDomainException(string entityName, T key) : base($"{entityName} {key} not found")
    {
        Key = key;
    }

    public override HttpStatusCode GetHttpStatusCode => HttpStatusCode.NotFound;
}

public partial class DomainException
{
    public static NotFoundDomainException<T> NotFound<T>(string entityName, T key) => new(entityName, key);
}