using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.CoreService.DataAccess;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.DTO.Abstractions;
using Notes.CoreService.Exceptions;

namespace Notes.CoreService.Domain.Notes;

/// <summary>
/// Данные для обновления заметки
/// </summary>
public class PatchNoteInput
{
    /// <summary>
    /// Название заметки
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Описание заметки
    /// </summary>
    public Optional<string> Description { get; set; }
}

public class PatchNoteInputValidator : AbstractValidator<PatchNoteInput>
{
    public PatchNoteInputValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(256)
            .When(x => x.Title != null);

        RuleFor(x => x.Description.Value)
            .NotEmpty()
            .MaximumLength(2048)
            .When(x => x.Description is { HasValue: false, Value: not null });
    }
}

public class PatchNoteCommand : IRequest
{
    public required Guid Id { get; init; }
    public required PatchNoteInput Input { get; init; }
}

public class PatchNoteCommandValidator : AbstractValidator<PatchNoteCommand>
{
    public PatchNoteCommandValidator(PatchNoteInputValidator patchNoteInputValidator)
    {
        RuleFor(x => x.Input)
            .SetValidator(patchNoteInputValidator);
    }
}

public class PatchNoteCommandHandler : IRequestHandler<PatchNoteCommand>
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public PatchNoteCommandHandler(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task Handle(PatchNoteCommand command, CancellationToken cancellationToken)
    {
        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);

        var note = await dbContext.Notes.Where(x => x.Id == command.Id).FirstOrDefaultAsync(cancellationToken)
                   ?? throw DomainException.NotFound(nameof(Note), new { command.Id });

        var input = command.Input;

        var updated = false;

        if (input.Title != null && note.Title != input.Title)
        {
            note.Title = input.Title;
            updated = true;
        }

        if (input.Description.HasValue && note.Description != input.Description.Value)
        {
            note.Description = input.Description.Value;
            updated = true;
        }

        if (updated)
        {
            note.Modified = DateTime.UtcNow;
            dbContext.Notes.Update(note);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}