using System.Net;

namespace Notes.CoreService.Exceptions;

public class UnauthorizedDomainException : DomainException
{
    public UnauthorizedDomainException(string? message) : base(message ?? "Unauthorized")
    {
    }

    public override HttpStatusCode GetHttpStatusCode => HttpStatusCode.Unauthorized;
}

public partial class DomainException
{
    public static UnauthorizedDomainException Unauthorized(string? message = null) => new(message);
}