using FluentValidation;
using MediatR;
using Notes.CoreService.Exceptions;

namespace Notes.CoreService.Services;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults =
                await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.Where(r => !r.IsValid).ToList();
            if (failures.Count != 0)
                throw DomainException.Validation(string.Join(Environment.NewLine, failures.Select(x => x.ToString())));
        }

        return await next();
    }
}