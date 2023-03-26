using FluentValidation;

namespace Notes.CoreService.DTO;

/// <summary>
/// Данные для создания заметки
/// </summary>
public class CreateNoteInput
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

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
    public CreateNoteInputValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2048)
            .When(x => x.Description != null);
    }
}