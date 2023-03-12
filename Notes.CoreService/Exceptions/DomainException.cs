using System.Net;

namespace Notes.CoreService.Exceptions;

public abstract partial class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    public abstract HttpStatusCode GetHttpStatusCode { get; }
}