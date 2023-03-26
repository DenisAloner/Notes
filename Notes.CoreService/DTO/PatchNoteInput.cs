using FluentValidation;
using Notes.CoreService.DTO.Abstractions;

namespace Notes.CoreService.DTO;

/// <summary>
/// Данные для обновления заметки
/// </summary>
public class PatchNoteInput
{
    /// <summary>
    /// Идентификатор заметки
    /// </summary>
    public Guid Id { get; set; }

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