using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.CoreService.DataAccess;
using Notes.CoreService.DataAccess.Entities;

namespace Notes.CoreService.Domain.Notes;

/// <summary>
/// Данные для создания заметки
/// </summary>
public class CreateNoteInput
{
    /// <summary>
    /// Название заметки
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Описание заметки
    /// </summary>
    public string? Description { get; set; }
}

public class CreateNoteInputValidator : AbstractValidator<CreateNoteInput>
{
    public CreateNoteInputValidator() {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2048)
            .When(x => x.Description != null);
    }
}

public class CreateNoteCommand : IRequest<Guid>
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

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Guid>
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public CreateNoteCommandHandler(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Guid> Handle(CreateNoteCommand command, CancellationToken cancellationToken)
    {
        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);

        var input = command.Input;

        var note = new Note
        {
            Title = input.Title,
            Description = input.Description,
            Modified = DateTime.UtcNow
        };

        await dbContext.Notes.AddAsync(note, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return note.Id;
    }
}