namespace Notes.CoreService.DTO;

/// <summary>
/// Данные для создания заметки
/// </summary>
public class CreateNoteRequest
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