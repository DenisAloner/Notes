using System.Net;

namespace Notes.CoreService.Exceptions;

public class ValidationDomainException: DomainException
{
    public ValidationDomainException(string message) : base(message)
    {
    }

    public override HttpStatusCode GetHttpStatusCode => HttpStatusCode.BadRequest;
}

public partial class DomainException
{
    public static ValidationDomainException Validation(string message) => new(message);
}