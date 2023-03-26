namespace Notes.CoreService.DTO;

/// <summary>
/// Заметка
/// </summary>
public class UserNote
{
    /// <summary>
    /// Идентификатор заметки
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название заметки
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Описание заметки
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Дата и время изменения заметки
    /// </summary>
    public DateTime? Modified { get; set; }
}