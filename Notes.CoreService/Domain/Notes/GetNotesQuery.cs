using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.CoreService.Abstractions;
using Notes.CoreService.DataAccess;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.Extensions;

namespace Notes.CoreService.Domain.Notes;

/// <summary>
/// Данные для получения списка заметок
/// </summary>
public class GetNotesInput : PageInput
{
}

public class GetNotesInputValidator : PageInputValidator<GetNotesInput>
{
}

public class GetNotesQuery : IRequest<Page<Note>>
{
    public required GetNotesInput Input { get; set; }
}

public class GetNotesQueryValidator : AbstractValidator<GetNotesQuery>
{
    public GetNotesQueryValidator(GetNotesInputValidator getNotesInputValidator)
    {
        RuleFor(x => x.Input)
            .SetValidator(getNotesInputValidator);
    }
}

public class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, Page<Note>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public GetNotesQueryHandler(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Page<Note>> Handle(GetNotesQuery query, CancellationToken cancellationToken)
    {
        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Notes.ToPageAsync(query.Input.PageNumber, query.Input.PageSize, cancellationToken);
    }
}