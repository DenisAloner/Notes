namespace Notes.CoreService.DTO;

/// <summary>
/// Данные о созданной заметке
/// </summary>
public class CreateNoteResponse
{
    /// <summary>
    /// Идентификатор заметки
    /// </summary>
    public required Guid Id { get; init; }
}