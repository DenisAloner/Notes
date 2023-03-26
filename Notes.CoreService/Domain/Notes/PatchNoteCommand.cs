using FluentValidation;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.CoreService.DataAccess;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.DTO;
using Notes.CoreService.Exceptions;

namespace Notes.CoreService.Domain.Notes;

public class PatchNoteCommand : IRequest
{
    public required Guid ModifiedBy { get; init; }
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

        var input = command.Input;

        var note = await dbContext.Notes
                       .Where(n => n.Id == input.Id)
                       .Select(n => new { n.Id, n.UserId })
                       .FirstOrDefaultAsyncLinqToDB(cancellationToken)
                   ?? throw DomainException.NotFound(nameof(Note), new { NoteId = input.Id });

        if (note.UserId != command.ModifiedBy) throw DomainException.Forbidden();

        var updatedNote = dbContext.Notes.Where(n => n.Id == input.Id).AsUpdatable();

        if (input.Title != null) updatedNote = updatedNote.Set(x => x.Title, input.Title);

        if (input.Description.HasValue) updatedNote = updatedNote.Set(x => x.Description, input.Description.Value);

        await updatedNote.UpdateAsync(cancellationToken);
    }
}