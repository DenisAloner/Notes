namespace Notes.CoreService.DataAccess.Entities;

/// <summary>
/// Заметка
/// </summary>
public class Note
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