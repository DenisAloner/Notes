using System.Net;

namespace Notes.CoreService.Exceptions;

public class ForbiddenDomainException : DomainException
{
    public ForbiddenDomainException(string? message) : base(message ?? "Forbidden")
    {
    }

    public override HttpStatusCode GetHttpStatusCode => HttpStatusCode.Forbidden;
}

public partial class DomainException
{
    public static ForbiddenDomainException Forbidden(string? message = null) => new(message);
}