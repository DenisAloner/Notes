using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.CoreService.DataAccess;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.DTO;

namespace Notes.CoreService.Domain.Notes;

public class CreateNoteCommand : IRequest<Note>
{
    public required CreateNoteInput Input { get; set; }
}

public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator(CreateNoteInputValidator createNoteInputValidator)
    {
        RuleFor(x => x.Input)
            .SetValidator(createNoteInputValidator);
    }
}

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Note>
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public CreateNoteCommandHandler(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Note> Handle(CreateNoteCommand command, CancellationToken cancellationToken)
    {
        var input = command.Input;

        var note = new Note
        {
            UserId = input.UserId,
            Title = input.Title,
            Description = input.Description,
            Modified = DateTime.UtcNow
        };

        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);
        await dbContext.Notes.AddAsync(note, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return note;
    }
}